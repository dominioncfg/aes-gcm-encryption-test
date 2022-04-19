namespace AesGcmTest.Infrastructure;

public interface ITenancySymmetricKeyRepository
{
    Task<PersistenceTenancyKeyModel?> GetByTenantIdOrDefaultAsync(Guid tenantId, CancellationToken cancellationToken);
    Task Add(PersistenceTenancyKeyModel tenantKey, CancellationToken cancellationToken);
}
