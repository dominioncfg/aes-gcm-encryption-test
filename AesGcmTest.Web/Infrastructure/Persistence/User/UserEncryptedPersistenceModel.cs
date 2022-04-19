namespace AesGcmTest.Infrastructure;

public class UserEncryptedPersistenceModel
{
    public Guid TenantId { get; set; }
    public Guid Id { get; set; }
    public byte[] EncryptedPayload { get; set; } = Array.Empty<byte>();
    public byte[] Nonce { get; set; } = Array.Empty<byte>();
    public byte[] Tag { get; set; } = Array.Empty<byte>();

    private UserEncryptedPersistenceModel(Guid tenantId, Guid id, byte[] encryptedPayload, byte[] tag, byte[] nonce)
    {
        TenantId = tenantId;
        Id = id;
        EncryptedPayload = encryptedPayload;
        Tag = tag;
        Nonce = nonce;
    }

    public static UserEncryptedPersistenceModel Create(Guid tenantId, Guid id, byte[] encryptedPayload, byte[] tag, byte[] nonce)
    {
        return new(tenantId, id, encryptedPayload, tag, nonce);
    }
}


