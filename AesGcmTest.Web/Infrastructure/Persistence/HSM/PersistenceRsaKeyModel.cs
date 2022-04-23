namespace AesGcmTest.Infrastructure;

public class PersistenceRsaKeyModel
{
    public Guid Id { get; }
    public string FriendlyKeyId { get; } = string.Empty;
    public byte[] PrivateKey { get; } = Array.Empty<byte>();
    public byte[] PublicKey { get; } = Array.Empty<byte>();
    public bool IsActive { get; private set; }

    protected PersistenceRsaKeyModel() { }

    private PersistenceRsaKeyModel(Guid id, string friendlyKeyId, byte[] privateKey, byte[] publicKey)
    {
        Id = id;
        FriendlyKeyId = friendlyKeyId;
        PrivateKey = privateKey;
        PublicKey = publicKey;
        IsActive = true;
    }

    public void Disable() => IsActive = false;

    public static PersistenceRsaKeyModel Create(string friendlyKeyId, byte[] privateKey, byte[] publicKey)
    {
        return new PersistenceRsaKeyModel(Guid.NewGuid(), friendlyKeyId, privateKey, publicKey);
    }
}
