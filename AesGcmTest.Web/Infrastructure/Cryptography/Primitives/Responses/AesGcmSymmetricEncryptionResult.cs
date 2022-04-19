namespace AesGcmTest.Infrastructure;

public record AesGcmSymmetricEncryptionResult
{
    public byte[] CipherTextInBytes { get; }
    public byte[] TagInBytes { get; }
    public AesGcmSymmetricEncryptionResult(byte[] chipherText, byte[] tag)
    {
        CipherTextInBytes = chipherText;
        TagInBytes = tag;
    }
}
