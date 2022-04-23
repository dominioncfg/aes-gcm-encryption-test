namespace AesGcmTest.Infrastructure;

public interface IRsaKeyLocalRepository
{
    public Task AddAsync(PersistenceRsaKeyModel rsaKey, CancellationToken cancellationToken);
    public Task<PersistenceRsaKeyModel> GetByFriendlyIdAsync(string rsaKeyId, CancellationToken cancellationToken);
    public Task DeleteAsync(PersistenceRsaKeyModel rsaKey, CancellationToken cancellationToken);
}
