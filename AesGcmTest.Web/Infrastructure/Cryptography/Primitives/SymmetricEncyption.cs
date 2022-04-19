using System.Security.Cryptography;

namespace AesGcmTest.Infrastructure;

public static class AesGcmSymmetricEncryption
{
    // * Consist of Aes + Galois Counter Mode
    // * Consist on Aes + MAC on the chiper text
    // * There is another AES CCM

    static readonly HashSet<int> allowedKeySizesInBytes = new()
    {
        16, //= 128 bits 
        24, //= 192 bits
        32, //= 256 bits 
    };
    const int NonceSizeInBytes = 12;
    const int TagSizeInBytes = 16;

    public static CryptographicKey GetRandomNonce()
    {
        //AES Gcm uses 12 bytes nonce (IV)
        return CryptographicKey.CreateRandomOfBytes(NonceSizeInBytes);
    }

    public static AesGcmSymmetricEncryptionResult Encrypt(byte[] plainTextBytes, byte[] key, byte[] nonce, byte[]? associatedData = null)
    {
        if (!allowedKeySizesInBytes.Contains(key.Length))
            throw new ArgumentException("The key has an invalid size");

        if (nonce.Length != NonceSizeInBytes)
            throw new ArgumentException("The nonce has an invalid size");

        using var aesGcm = new AesCcm(key);

        var tag = new byte[TagSizeInBytes];
        var chipherText = new byte[plainTextBytes.Length];

        aesGcm.Encrypt(nonce, plainTextBytes, chipherText, tag, associatedData);

        return new AesGcmSymmetricEncryptionResult(chipherText, tag);
    }

    public static AesGcmSymmetricDecryptionResult Decrypt(byte[] cipherText, byte[] key, byte[] nonce, byte[] tag, byte[]? associatedData = null)
    {
        if (!allowedKeySizesInBytes.Contains(key.Length))
            throw new ArgumentException("The key has an invalid size");

        if (nonce.Length != NonceSizeInBytes)
            throw new ArgumentException("The nonce has an invalid size");

        if (tag.Length != TagSizeInBytes)
            throw new ArgumentException("The tag has an invalid size");

        var plainTextBytes = new byte[cipherText.Length];

        using var aesGcm = new AesCcm(key);
        aesGcm.Decrypt(nonce, cipherText, tag, plainTextBytes, associatedData);

        return new AesGcmSymmetricDecryptionResult(plainTextBytes);
    }
}