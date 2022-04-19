namespace AesGcmTest.Infrastructure;

public class InMemoryTenancySymmetricKeyRepository : ITenancySymmetricKeyRepository
{
    private readonly Dictionary<Guid, PersistenceTenancyKeyModel> _tenantsStorage;
    public InMemoryTenancySymmetricKeyRepository(Dictionary<Guid, PersistenceTenancyKeyModel> tenantsStorage)
    {
        _tenantsStorage = tenantsStorage;
    }

    public Task<PersistenceTenancyKeyModel?> GetByTenantIdOrDefaultAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenantKey = _tenantsStorage.ContainsKey(tenantId) ? _tenantsStorage[tenantId] : null;
        return Task.FromResult(tenantKey);
    }
    public Task Add(PersistenceTenancyKeyModel tenantKey, CancellationToken cancellationToken)
    {
        _tenantsStorage.Add(tenantKey.TenantId, tenantKey);
        return Task.CompletedTask;
    }
}
