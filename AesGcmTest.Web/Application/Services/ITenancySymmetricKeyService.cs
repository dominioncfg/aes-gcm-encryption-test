namespace AesGcmTest.Application;

public interface ITenancySymmetricKeyService
{
    Task<byte[]> GetOrCreateTenantSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken);

    Task<byte[]> GetExistingTenantSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken);

    Task RotateSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken);
}
