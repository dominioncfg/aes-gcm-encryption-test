namespace AesGcmTest.Infrastructure;

public class UserEncryptedPersistenceModel
{
    public Guid TenantId { get; }
    public Guid Id { get; }
    public byte[] EncryptedPayload { get; } = Array.Empty<byte>();

    private UserEncryptedPersistenceModel(Guid tenantId, Guid id, byte[] encryptedPayload)
    {
        TenantId = tenantId;
        Id = id;
        EncryptedPayload = encryptedPayload;
    }

    protected UserEncryptedPersistenceModel() { }

    public static UserEncryptedPersistenceModel Create(Guid tenantId, Guid id, byte[] encryptedPayload)
    {
        return new(tenantId, id, encryptedPayload);
    }
}
