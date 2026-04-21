using Domain.Abstractions;

namespace Infrastructure.Services;

public class DummyEncryptionService : IEncryptionService
{
    public string Encrypt(string plainText)
    {
        // Şimdilik gerçek şifreleme yok
        return plainText;
    }

    public string Decrypt(string cipherText)
    {
        return cipherText;
    }
}
