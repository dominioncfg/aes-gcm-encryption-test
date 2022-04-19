namespace AesGcmTest.Infrastructure;

public record GenerateWrappedSymmetricKeyResponse
{
    public byte[] SymmetricKeyPlainTextInBytes { get; init; } = Array.Empty<byte>();
    public byte[] SymmetricKeyCipherTextInBytes { get; init; } = Array.Empty<byte>();
}
