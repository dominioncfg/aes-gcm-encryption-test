namespace AesGcmTest.Infrastructure;

public record ASymmetricDecryptionResult
{
    public byte[] PlainTextInBytes { get; }
    public ASymmetricDecryptionResult(byte[] plainTextInBytes) => PlainTextInBytes = plainTextInBytes;
}
