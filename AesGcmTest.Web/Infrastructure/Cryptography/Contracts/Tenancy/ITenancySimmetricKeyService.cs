namespace AesGcmTest.Infrastructure;

public interface ITenancySimmetricKeyService
{
    Task<byte[]> GetOrCreateTenantSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken);

    Task<byte[]> GetExistingTenantSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken);
}
