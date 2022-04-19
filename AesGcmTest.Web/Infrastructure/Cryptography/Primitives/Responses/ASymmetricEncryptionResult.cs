namespace AesGcmTest.Infrastructure;

public record ASymmetricEncryptionResult
{
    public byte[] ChipherTextInBytes { get; }
    public ASymmetricEncryptionResult(byte[] bytes) => ChipherTextInBytes = bytes;
}
