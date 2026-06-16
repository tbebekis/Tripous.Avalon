namespace Tripous.Tests;

public class SecTests
{
    [Fact]
    public void EncryptDecrypt_ReturnsOriginalText()
    {
        string Salt = Sec.CreateSalt();
        string EncryptedText = Sec.Encrypt("Secret text", "password", Salt, 10000);

        string Result = Sec.Decrypt(EncryptedText, "password", Salt, 10000);

        Assert.Equal("Secret text", Result);
    }
    [Fact]
    public void Decrypt_ThrowsWhenCipherTextIsTampered()
    {
        string Salt = Sec.CreateSalt();
        string EncryptedText = Sec.Encrypt("Secret text", "password", Salt, 10000);
        byte[] Bytes = Convert.FromBase64String(EncryptedText);
        Bytes[Bytes.Length - 1] ^= 1;
        string TamperedText = Convert.ToBase64String(Bytes);

        Assert.Throws<System.Security.Cryptography.AuthenticationTagMismatchException>(() => Sec.Decrypt(TamperedText, "password", Salt, 10000));
    }
}
