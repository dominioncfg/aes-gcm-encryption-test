namespace AesGcmTest.Infrastructure;

public record PersistenceTenancyKeyModel
{
    public Guid TenantId { get; init; }
    public string HsmKeyId { get; init; } = string.Empty;
    public byte[] AesGcmEncryptedKey { get; init; } = Array.Empty<byte>();
    public bool IsActive { get; set; }
}
