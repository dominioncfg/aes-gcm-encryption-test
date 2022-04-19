namespace AesGcmTest.Infrastructure;

public record UnwrapTenantSymmetricKeyRequest
{
    public string TenantRsaKeyId { get; init; } = string.Empty;
    public byte[] WrappedSymmetricKeyCipherTextInBytes { get; init; } = Array.Empty<byte>();
}
