namespace AesGcmTest.Infrastructure;

public class UserEncryptedPersistenceModel
{
    public Guid TenantId { get; set; }
    public Guid Id { get; set; }
    public byte[] EncryptedPayload { get; set; } = Array.Empty<byte>();

    private UserEncryptedPersistenceModel(Guid tenantId, Guid id, byte[] encryptedPayload)
    {
        TenantId = tenantId;
        Id = id;
        EncryptedPayload = encryptedPayload;
    }

    public static UserEncryptedPersistenceModel Create(Guid tenantId, Guid id, byte[] encryptedPayload)
    {
        return new(tenantId, id, encryptedPayload);
    }
}


