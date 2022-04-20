namespace AesGcmTest.Infrastructure;

public record WrapSymmetricKeyResponse
{
    public byte[] SymmetricKeyCipherTextInBytes { get; init; } = Array.Empty<byte>();
}
