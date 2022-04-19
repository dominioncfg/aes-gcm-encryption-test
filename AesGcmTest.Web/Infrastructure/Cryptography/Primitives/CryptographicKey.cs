using System.Security.Cryptography;

namespace AesGcmTest.Infrastructure;

public record CryptographicKey
{
    public byte[] Bytes { get; }
    public string String => Convert.ToBase64String(Bytes);
    public CryptographicKey(byte[] bytes) => Bytes = bytes;

    public static CryptographicKey CreateRandomOfBytes(int bytesCount) => new(RandomNumberGenerator.GetBytes(bytesCount));
}