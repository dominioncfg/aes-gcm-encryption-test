namespace AesGcmTest.Infrastructure;

public record AuthenticatedEncryptionDecryptRequest
{
    public string SchemaVersion { get; init; } = string.Empty;
    public byte[] SymmetricKey { get; init; } = Array.Empty<byte>();
    public byte[] ComposedEncryptedPayload { get; init; } = Array.Empty<byte>();
}
