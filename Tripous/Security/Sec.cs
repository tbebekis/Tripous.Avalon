namespace Tripous;

/// <summary>
/// Provides security helper functions for password hashing, key derivation and encryption.
/// </summary>
static public class Sec
{
 
    // ● public
    /// <summary>
    /// Creates a cryptographically secure random salt value.
    /// </summary>
    static public string CreateSalt()
    {
        byte[] SaltBytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(SaltBytes);
    }
    /// <summary>
    /// Derives an encryption key from a password.
    /// </summary>
    static public byte[] DeriveKey(string PasswordPlainText, string SaltBase64, int Iterations)
    {
        byte[] SaltBytes = Convert.FromBase64String(SaltBase64);
        
        return Rfc2898DeriveBytes.Pbkdf2(
            PasswordPlainText,
            SaltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            32); //   (output length)
    }
    /// <summary>
    /// Creates a password hash using PBKDF2.
    /// </summary>
    static public string HashPassword(string PasswordPlainText, string SaltBase64, int Iterations)
    {
        byte[] HashBytes = DeriveKey(PasswordPlainText, SaltBase64, Iterations); 
        return Convert.ToBase64String(HashBytes);
    }
    /// <summary>
    /// Verifies that a password matches a stored hash.
    /// </summary>
    static public bool VerifyPassword(string PasswordPlainText, string PasswordHashBase64, string SaltBase64, int Iterations)
    {
        string HashBase64 = HashPassword(PasswordPlainText, SaltBase64, Iterations);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(HashBase64),
            Convert.FromBase64String(PasswordHashBase64));
    }
    /// <summary>
    /// Encrypts a plain text string and returns a Base64 encoded result.
    /// </summary>
    static public string Encrypt(string PlainText, string PasswordPlainText, string SaltBase64, int Iterations)
    {
        byte[] Key = DeriveKey(PasswordPlainText, SaltBase64, Iterations);
        byte[] PlainBytes = Encoding.UTF8.GetBytes(PlainText);
        byte[] NonceBytes = RandomNumberGenerator.GetBytes(12);
        byte[] TagBytes = new byte[16];
        byte[] CipherBytes = new byte[PlainBytes.Length];

        using (AesGcm AesCipher = new AesGcm(Key, TagBytes.Length))
        {
            AesCipher.Encrypt(NonceBytes, PlainBytes, CipherBytes, TagBytes);
        }

        byte[] ResultBytes = new byte[NonceBytes.Length + TagBytes.Length + CipherBytes.Length];
        Buffer.BlockCopy(NonceBytes, 0, ResultBytes, 0, NonceBytes.Length);
        Buffer.BlockCopy(TagBytes, 0, ResultBytes, NonceBytes.Length, TagBytes.Length);
        Buffer.BlockCopy(CipherBytes, 0, ResultBytes, NonceBytes.Length + TagBytes.Length, CipherBytes.Length);
        return Convert.ToBase64String(ResultBytes);
    }
    /// <summary>
    /// Decrypts a Base64 encoded encrypted string and returns the original plain text.
    /// </summary>
    static public string Decrypt(string CipherTextBase64, string PasswordPlainText, string SaltBase64, int Iterations)
    {
        byte[] Key = DeriveKey(PasswordPlainText, SaltBase64, Iterations);
        byte[] SourceBytes = Convert.FromBase64String(CipherTextBase64);
        const int NonceLength = 12;
        const int TagLength = 16;

        if (SourceBytes.Length < NonceLength + TagLength)
            throw new CryptographicException("Invalid encrypted payload.");

        byte[] NonceBytes = new byte[NonceLength];
        byte[] TagBytes = new byte[TagLength];
        byte[] CipherBytes = new byte[SourceBytes.Length - NonceLength - TagLength];
        byte[] PlainBytes = new byte[CipherBytes.Length];

        Buffer.BlockCopy(SourceBytes, 0, NonceBytes, 0, NonceBytes.Length);
        Buffer.BlockCopy(SourceBytes, NonceBytes.Length, TagBytes, 0, TagBytes.Length);
        Buffer.BlockCopy(SourceBytes, NonceBytes.Length + TagBytes.Length, CipherBytes, 0, CipherBytes.Length);

        using (AesGcm AesCipher = new AesGcm(Key, TagBytes.Length))
        {
            AesCipher.Decrypt(NonceBytes, CipherBytes, TagBytes, PlainBytes);
        }

        return Encoding.UTF8.GetString(PlainBytes);
    }
}
