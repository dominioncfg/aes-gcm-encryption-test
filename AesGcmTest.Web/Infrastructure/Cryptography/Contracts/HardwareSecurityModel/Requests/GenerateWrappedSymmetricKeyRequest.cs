namespace AesGcmTest.Infrastructure;

public record GenerateWrappedSymmetricKeyRequest
{
    public string TenantRsaKeyId { get; init; } = string.Empty;
    public int SymmetricKeyLengthInBytes { get; init; } = 32;
}
