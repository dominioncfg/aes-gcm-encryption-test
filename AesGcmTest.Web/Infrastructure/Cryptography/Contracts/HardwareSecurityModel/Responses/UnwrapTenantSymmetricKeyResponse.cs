namespace AesGcmTest.Infrastructure;

public record UnwrapTenantSymmetricKeyResponse
{
    public byte[] SymmetricKeyPlainTextInBytes { get; init; } = Array.Empty<byte>();
}
