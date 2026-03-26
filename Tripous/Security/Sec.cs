using System.Security.Cryptography;
using System.Text;

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
    static public bool VerifyPassword(string PasswordPlainText, string StoredHashBase64, string SaltBase64, int Iterations)
    {
        string HashBase64 = HashPassword(PasswordPlainText, SaltBase64, Iterations);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(HashBase64),
            Convert.FromBase64String(StoredHashBase64));
    }
    /// <summary>
    /// Encrypts a plain text string and returns a Base64 encoded result.
    /// </summary>
    static public string Encrypt(string PlainText, string PasswordPlainText, string SaltBase64, int Iterations)
    {
        byte[] Key = DeriveKey(PasswordPlainText, SaltBase64, Iterations);
        byte[] PlainBytes = Encoding.UTF8.GetBytes(PlainText);

        using (Aes AesCipher = Aes.Create())
        {
            AesCipher.Key = Key;
            AesCipher.GenerateIV();

            using (ICryptoTransform Encryptor = AesCipher.CreateEncryptor(AesCipher.Key, AesCipher.IV))
            {
                byte[] CipherBytes = Encryptor.TransformFinalBlock(PlainBytes, 0, PlainBytes.Length);

                byte[] ResultBytes = new byte[AesCipher.IV.Length + CipherBytes.Length];

                Buffer.BlockCopy(AesCipher.IV, 0, ResultBytes, 0, AesCipher.IV.Length);
                Buffer.BlockCopy(CipherBytes, 0, ResultBytes, AesCipher.IV.Length, CipherBytes.Length);

                return Convert.ToBase64String(ResultBytes);
            }
        }
    }
    /// <summary>
    /// Decrypts a Base64 encoded encrypted string and returns the original plain text.
    /// </summary>
    static public string Decrypt(string CipherTextBase64, string PasswordPlainText, string SaltBase64, int Iterations)
    {
        byte[] Key = DeriveKey(PasswordPlainText, SaltBase64, Iterations);
        byte[] FullCipherBytes = Convert.FromBase64String(CipherTextBase64);

        using (Aes AesCipher = Aes.Create())
        {
            AesCipher.Key = Key;

            int IvLength = AesCipher.BlockSize / 8;

            byte[] IvBytes = new byte[IvLength];
            byte[] CipherBytes = new byte[FullCipherBytes.Length - IvLength];

            Buffer.BlockCopy(FullCipherBytes, 0, IvBytes, 0, IvLength);
            Buffer.BlockCopy(FullCipherBytes, IvLength, CipherBytes, 0, CipherBytes.Length);

            AesCipher.IV = IvBytes;

            using (ICryptoTransform Decryptor = AesCipher.CreateDecryptor(AesCipher.Key, AesCipher.IV))
            {
                byte[] PlainBytes = Decryptor.TransformFinalBlock(CipherBytes, 0, CipherBytes.Length);
                return Encoding.UTF8.GetString(PlainBytes);
            }
        }
    }
}