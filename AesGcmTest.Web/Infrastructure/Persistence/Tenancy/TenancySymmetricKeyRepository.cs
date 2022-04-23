using Microsoft.EntityFrameworkCore;

namespace AesGcmTest.Infrastructure;

public class TenancySymmetricKeyRepository : ITenancySymmetricKeyRepository
{
    private readonly AesGcmDbContext _dbContext;

    public TenancySymmetricKeyRepository(AesGcmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PersistenceTenancyKeyModel?> GetByTenantIdOrDefaultAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenantKey = await _dbContext.TenantsSymmetricKeys.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive, cancellationToken);
        return tenantKey;
    }

    public async Task AddAsync(PersistenceTenancyKeyModel tenantKey, CancellationToken cancellationToken)
    {
        await _dbContext.TenantsSymmetricKeys.AddAsync(tenantKey, cancellationToken);
    }

    public Task UpdateAsync(PersistenceTenancyKeyModel tenantKey, CancellationToken cancellationToken)
    {
        _dbContext.TenantsSymmetricKeys.Update(tenantKey);
        return Task.CompletedTask;
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
