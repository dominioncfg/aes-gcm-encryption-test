namespace AesGcmTest.Infrastructure;

public record AuthenticatedEncryptionEncryptResponse
{
    public byte[] ComposedEncryptedPayload { get; init; } = Array.Empty<byte>();
}
