namespace PasswordManager.Services;

/// <summary>
/// Provides master password validation and field encryption services.
/// </summary>
static public class VaultService
{
    // ● private fields
    const string Prefix = "v1:";
    static byte[] fKey;

    // ● private methods
    /// <summary>
    /// Derives a cryptographic key from a password and salt.
    /// </summary>
    static byte[] DeriveKey(string Password, byte[] Salt, int Iterations)
    {
        return Rfc2898DeriveBytes.Pbkdf2(Password, Salt, Iterations, HashAlgorithmName.SHA256, 32);
    }
    /// <summary>
    /// Creates a verifier hash from a derived key without storing the key itself.
    /// </summary>
    static byte[] CreateVerifier(byte[] Key)
    {
        byte[] PrefixBytes = Encoding.UTF8.GetBytes("PasswordManager.MasterVerifier.v1");
        byte[] Payload = new byte[PrefixBytes.Length + Key.Length];
        Buffer.BlockCopy(PrefixBytes, 0, Payload, 0, PrefixBytes.Length);
        Buffer.BlockCopy(Key, 0, Payload, PrefixBytes.Length, Key.Length);
        return SHA256.HashData(Payload);
    }
    /// <summary>
    /// Returns true when the specified text contains an uppercase letter.
    /// </summary>
    static bool HasUppercase(string Text) => Text.Any(char.IsUpper);
    /// <summary>
    /// Returns true when the specified text contains a lowercase letter.
    /// </summary>
    static bool HasLowercase(string Text) => Text.Any(char.IsLower);
    /// <summary>
    /// Returns true when the specified text contains a digit.
    /// </summary>
    static bool HasDigit(string Text) => Text.Any(char.IsDigit);
    /// <summary>
    /// Returns the configured integer value.
    /// </summary>
    static int GetConfigInteger(string Name, int DefaultValue)
    {
        string Value = Config.GetValue(Name);
        return int.TryParse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Result) ? Result : DefaultValue;
    }
    /// <summary>
    /// Returns true when the specified text contains a valid Base64 value with the expected byte length.
    /// </summary>
    static bool IsValidBase64(string Text, int ByteLength)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return false;
        try
        {
            return Convert.FromBase64String(Text).Length == ByteLength;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    // ● static public
    /// <summary>
    /// Returns true when a master password verifier exists.
    /// </summary>
    static public bool HasMasterPassword()
    {
        string SaltText = Config.GetValue("PasswordManager.MasterSalt");
        string HashText = Config.GetValue("PasswordManager.MasterHash");
        return IsValidBase64(SaltText, 16) && IsValidBase64(HashText, 32);
    }
    /// <summary>
    /// Validates the sample master password policy and returns an error message.
    /// </summary>
    static public bool ValidateMasterPassword(string Password, out string Message)
    {
        int MinimumLength = GetConfigInteger("PasswordManager.MinimumPasswordLength", 8);
        if (string.IsNullOrWhiteSpace(Password) || Password.Length < MinimumLength)
        {
            Message = $"The master password must be at least {MinimumLength} characters long.";
            return false;
        }
        if (!HasUppercase(Password))
        {
            Message = "The master password must contain at least one uppercase letter.";
            return false;
        }
        if (!HasLowercase(Password))
        {
            Message = "The master password must contain at least one lowercase letter.";
            return false;
        }
        if (!HasDigit(Password))
        {
            Message = "The master password must contain at least one digit.";
            return false;
        }

        Message = string.Empty;
        return true;
    }
    /// <summary>
    /// Creates and stores the master password verifier.
    /// </summary>
    static public void CreateMasterPassword(string Password)
    {
        int Iterations = GetConfigInteger("PasswordManager.KdfIterations", 100000);
        byte[] Salt = RandomNumberGenerator.GetBytes(16);
        byte[] Key = DeriveKey(Password, Salt, Iterations);
        byte[] Verifier = CreateVerifier(Key);
        Config.SetValue("PasswordManager.MasterSalt", Convert.ToBase64String(Salt), ConfigScope.System, "");
        Config.SetValue("PasswordManager.MasterHash", Convert.ToBase64String(Verifier), ConfigScope.System, "");
        Config.SetValue("PasswordManager.KdfIterations", Iterations.ToString(CultureInfo.InvariantCulture), ConfigScope.System, "");
        fKey = Key;
    }
    /// <summary>
    /// Unlocks the vault using the specified master password.
    /// </summary>
    static public bool Unlock(string Password)
    {
        if (string.IsNullOrEmpty(Password))
            return false;
        string SaltText = Config.GetValue("PasswordManager.MasterSalt");
        string HashText = Config.GetValue("PasswordManager.MasterHash");
        int Iterations = GetConfigInteger("PasswordManager.KdfIterations", 100000);
        if (string.IsNullOrWhiteSpace(SaltText) || string.IsNullOrWhiteSpace(HashText))
            return false;
        byte[] Salt;
        byte[] StoredVerifier;
        try
        {
            Salt = Convert.FromBase64String(SaltText);
            StoredVerifier = Convert.FromBase64String(HashText);
        }
        catch (FormatException)
        {
            return false;
        }
        byte[] Key = DeriveKey(Password, Salt, Iterations);
        byte[] Verifier = CreateVerifier(Key);
        bool Result = CryptographicOperations.FixedTimeEquals(Verifier, StoredVerifier);
        if (Result)
            fKey = Key;
        else
            CryptographicOperations.ZeroMemory(Key);
        return Result;
    }
    /// <summary>
    /// Locks the vault and forgets the runtime key.
    /// </summary>
    static public void Lock()
    {
        if (fKey != null)
            CryptographicOperations.ZeroMemory(fKey);
        fKey = null;
    }
    /// <summary>
    /// Encrypts a plaintext value.
    /// </summary>
    static public string Encrypt(string PlainText)
    {
        if (string.IsNullOrEmpty(PlainText))
            return string.Empty;
        if (PlainText.StartsWith(Prefix, StringComparison.Ordinal))
            return PlainText;
        if (fKey == null)
            throw new TripousException("The vault is locked.");
        byte[] Nonce = RandomNumberGenerator.GetBytes(12);
        byte[] PlainBytes = Encoding.UTF8.GetBytes(PlainText);
        byte[] CipherBytes = new byte[PlainBytes.Length];
        byte[] Tag = new byte[16];
        using AesGcm Aes = new(fKey, 16);
        Aes.Encrypt(Nonce, PlainBytes, CipherBytes, Tag);
        byte[] Payload = new byte[Nonce.Length + Tag.Length + CipherBytes.Length];
        Buffer.BlockCopy(Nonce, 0, Payload, 0, Nonce.Length);
        Buffer.BlockCopy(Tag, 0, Payload, Nonce.Length, Tag.Length);
        Buffer.BlockCopy(CipherBytes, 0, Payload, Nonce.Length + Tag.Length, CipherBytes.Length);
        return Prefix + Convert.ToBase64String(Payload);
    }
    /// <summary>
    /// Decrypts an encrypted value.
    /// </summary>
    static public string Decrypt(string CipherText)
    {
        if (string.IsNullOrEmpty(CipherText))
            return string.Empty;
        if (!CipherText.StartsWith(Prefix, StringComparison.Ordinal))
            return CipherText;
        if (fKey == null)
            throw new TripousException("The vault is locked.");
        byte[] Payload = Convert.FromBase64String(CipherText[Prefix.Length..]);
        byte[] Nonce = Payload[..12];
        byte[] Tag = Payload[12..28];
        byte[] CipherBytes = Payload[28..];
        byte[] PlainBytes = new byte[CipherBytes.Length];
        using AesGcm Aes = new(fKey, 16);
        Aes.Decrypt(Nonce, CipherBytes, Tag, PlainBytes);
        return Encoding.UTF8.GetString(PlainBytes);
    }
    /// <summary>
    /// Generates a random password for manual use by the user.
    /// </summary>
    static public string GeneratePassword(int Length = 20)
    {
        const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
        char[] Result = new char[Length];
        byte[] Bytes = RandomNumberGenerator.GetBytes(Length);
        for (int i = 0; i < Result.Length; i++)
            Result[i] = Chars[Bytes[i] % Chars.Length];
        return new string(Result);
    }

    // ● properties
    /// <summary>
    /// Gets true when the vault is unlocked.
    /// </summary>
    static public bool IsUnlocked => fKey != null;
}
