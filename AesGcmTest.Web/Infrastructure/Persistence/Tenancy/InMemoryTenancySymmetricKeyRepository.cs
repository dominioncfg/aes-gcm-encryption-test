namespace AesGcmTest.Infrastructure;

public class InMemoryTenancySymmetricKeyRepository : ITenancySymmetricKeyRepository
{
    private readonly List<PersistenceTenancyKeyModel> _tenantsStorage;
    public InMemoryTenancySymmetricKeyRepository(List<PersistenceTenancyKeyModel> tenantsStorage)
    {
        _tenantsStorage = tenantsStorage;
    }

    public Task<PersistenceTenancyKeyModel?> GetByTenantIdOrDefaultAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenantKey = _tenantsStorage.FirstOrDefault(x => x.TenantId == tenantId && x.IsActive);
        return Task.FromResult(tenantKey);
    }

    public Task AddAsync(PersistenceTenancyKeyModel tenantKey, CancellationToken cancellationToken)
    {
        _tenantsStorage.Add(tenantKey);
        return Task.CompletedTask;
    }

    public async Task RotateAsync(PersistenceTenancyKeyModel tenantKey, CancellationToken cancellationToken)
    {
        var oldKey = await GetByTenantIdOrDefaultAsync(tenantKey.TenantId, cancellationToken);
        if(oldKey is not null)
            oldKey.IsActive = false;


        _tenantsStorage.Add(tenantKey);
    }
}
