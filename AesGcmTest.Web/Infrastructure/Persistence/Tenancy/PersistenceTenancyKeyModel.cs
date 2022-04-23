namespace AesGcmTest.Infrastructure;

public class PersistenceTenancyKeyModel
{
    public Guid Id { get; }
    public Guid TenantId { get; }
    public string HsmKeyId { get; } = string.Empty;
    public byte[] AesGcmEncryptedKey { get; } = Array.Empty<byte>();
    public bool IsActive { get; private set; }

    private PersistenceTenancyKeyModel(Guid id, Guid tenantId, string hsmKeyId, byte[] aesGcmEncryptedKey)
    {
        Id = id;
        TenantId = tenantId;
        HsmKeyId = hsmKeyId;
        AesGcmEncryptedKey = aesGcmEncryptedKey;
        IsActive = true;
    }

    protected PersistenceTenancyKeyModel() { }

    public static PersistenceTenancyKeyModel CreateTenantNewKey(Guid tenantId, string keyId, byte[] aesGcmEncryptedKey)
    {
        return new PersistenceTenancyKeyModel(new Guid(), tenantId, keyId, aesGcmEncryptedKey);
    }

    public void Disable() => IsActive = false;
}
