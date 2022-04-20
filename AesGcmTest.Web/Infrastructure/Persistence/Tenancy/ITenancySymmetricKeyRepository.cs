namespace AesGcmTest.Infrastructure;

public interface ITenancySymmetricKeyRepository
{
    Task<PersistenceTenancyKeyModel?> GetByTenantIdOrDefaultAsync(Guid tenantId, CancellationToken cancellationToken);
    Task AddAsync(PersistenceTenancyKeyModel tenantKey, CancellationToken cancellationToken);
    Task RotateAsync(PersistenceTenancyKeyModel tenantKey, CancellationToken cancellationToken);
}
