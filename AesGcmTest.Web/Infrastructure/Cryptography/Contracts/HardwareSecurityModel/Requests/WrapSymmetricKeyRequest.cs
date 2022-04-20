namespace AesGcmTest.Infrastructure;

public record WrapSymmetricKeyRequest
{
    public string TenantRsaKeyId { get; init; } = string.Empty;
    public byte[] SymmetricKey { get; init; } = Array.Empty<byte>();
}
