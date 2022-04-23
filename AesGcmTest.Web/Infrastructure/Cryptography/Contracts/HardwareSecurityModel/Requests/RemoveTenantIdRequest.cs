namespace AesGcmTest.Infrastructure;

public record RemoveTenantIdRequest
{
    public string TenantRsaKeyId { get; init; } = string.Empty;
}
