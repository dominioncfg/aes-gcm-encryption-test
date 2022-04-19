namespace AesGcmTest.Infrastructure;

public record AesGcmSymmetricDecryptionResult
{
    public byte[] PlainTextInBytes { get; }
    public AesGcmSymmetricDecryptionResult(byte[] plainText) => PlainTextInBytes = plainText;
}
