namespace AesGcmTest.Infrastructure;

public record GenerateTenantRsaKeyResponse
{
    public string TenantRsaKeyId { get; init; } = string.Empty;
}
