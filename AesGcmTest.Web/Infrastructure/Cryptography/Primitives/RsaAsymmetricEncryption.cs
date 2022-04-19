using System.Security.Cryptography;

namespace AesGcmTest.Infrastructure;

public static class RsaAsymmetricEncryption
{
    // * Tech developed by a company name RSA and based on prime numbers
    // * Can only encrypt data smaller than the key
    // * Common key sizes are 1024 (128 bytes), 2048 (256 bytes), 4096 (512 bytes) bits. Use At least 2048.
    // * The size of the key affects performance
    
    public static AsymmetricEncryptionKeyPairResult CreateKeyPair(int keyLengthInBytes)
    {
        using var rsa = RSA.Create(keyLengthInBytes);
        var privateKey = rsa.ExportRSAPrivateKey();
        var publicKey = rsa.ExportRSAPublicKey();
        return new AsymmetricEncryptionKeyPairResult(privateKey, publicKey);
    }

    public static ASymmetricEncryptionResult Encrypt(byte[] plainText, byte[] publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);

        var chiperTextBytes = rsa.Encrypt(plainText, RSAEncryptionPadding.OaepSHA256);
        return new ASymmetricEncryptionResult(chiperTextBytes);
    }

    public static ASymmetricDecryptionResult Decrypt(byte[] chiperText, byte[] privateKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);

        var chiperTextBytes = rsa.Decrypt(chiperText, RSAEncryptionPadding.OaepSHA256);

        return new ASymmetricDecryptionResult(chiperTextBytes);
    }
}