namespace AesGcmTest.Infrastructure;

public record AsymmetricEncryptionKeyPairResult
{
    public byte[] PrivateKeyInBytes { get; }
    public string PrivateKeyString => Convert.ToBase64String(PrivateKeyInBytes);

    public byte[] PublicKeyInBytes { get; }
    public string PublicKeyString => Convert.ToBase64String(PublicKeyInBytes);

    public AsymmetricEncryptionKeyPairResult(byte[] privateKey, byte[] publicKey)
    {
        PrivateKeyInBytes = privateKey;
        PublicKeyInBytes = publicKey;
    }
}
