namespace AesGcmTest.Infrastructure;

public interface ITenancySimmetricKeyService
{
    Task<byte[]> GetOrCreateTenantSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken);
}
