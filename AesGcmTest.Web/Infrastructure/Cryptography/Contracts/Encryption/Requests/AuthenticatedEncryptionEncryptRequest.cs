namespace AesGcmTest.Infrastructure;

public record AuthenticatedEncryptionEncryptRequest
{
    public string SchemaVersion { get; init; } = string.Empty;
    public byte[] SymmetricKey { get; init; } = Array.Empty<byte>();
    public byte[] Nonce { get; init; } = Array.Empty<byte>();
    public object PayLoad { get; init; } = new();
}
