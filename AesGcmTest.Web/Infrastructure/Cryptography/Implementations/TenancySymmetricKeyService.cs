using AesGcmTest.Application;

namespace AesGcmTest.Infrastructure;

public class TenancySymmetricKeyService : ITenancySymmetricKeyService
{
    private readonly ITenancyKeyHardwareSecurityModuleService _hsm;
    private readonly ITenancySymmetricKeyRepository _keysRepo;
    private const int SymmetricAesGcmKeyLengthInBytes = 32;

    public TenancySymmetricKeyService(ITenancyKeyHardwareSecurityModuleService hsm, ITenancySymmetricKeyRepository keysRepo)
    {
        _hsm = hsm;
        _keysRepo = keysRepo;
    }

    public async Task<byte[]> GetOrCreateTenantSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenantEncryptionModel = await _keysRepo.GetByTenantIdOrDefaultAsync(tenantId, cancellationToken);

        if (tenantEncryptionModel is not null)
            return await UnwrapEncryptedKeyAsync(tenantEncryptionModel.HsmKeyId, tenantEncryptionModel.AesGcmEncryptedKey, cancellationToken);


        var hsmKeyId = await GenerateNewRsaKeyInHsmForTenant(cancellationToken);
        var symmetricKey = await GenerateNewAesGcmEncryptionKeyForTenant(hsmKeyId, cancellationToken);
        await SaveNewEncryptedSymmetricEncryptionKey(tenantId, hsmKeyId, symmetricKey, cancellationToken);
        return symmetricKey.SymmetricKeyPlainTextInBytes;

    }

    public async Task<byte[]> GetExistingTenantSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenantEncryptionModel = await _keysRepo.GetByTenantIdOrDefaultAsync(tenantId, cancellationToken);

        if (tenantEncryptionModel is null)
            throw new Exception($"Key for tenant {tenantId} dont exist");

        return await UnwrapEncryptedKeyAsync(tenantEncryptionModel.HsmKeyId, tenantEncryptionModel.AesGcmEncryptedKey, cancellationToken);
    }

    public async Task RotateSymmetricEncryptionKeyAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var existingUnwrappedTenantKey = await GetExistingTenantSymmetricEncryptionKeyAsync(tenantId, cancellationToken);
        var tenantNewRsaKeyId = await GenerateNewRsaKeyInHsmForTenant(cancellationToken);
        var request = new WrapSymmetricKeyRequest()
        {
            TenantRsaKeyId = tenantNewRsaKeyId,
            SymmetricKey = existingUnwrappedTenantKey,
        };
        var keyResponse = await _hsm.WrapSymmetricKeyAsync(request, cancellationToken);
        var storageModel = new PersistenceTenancyKeyModel()
        {
            TenantId = tenantId,
            HsmKeyId = tenantNewRsaKeyId,
            AesGcmEncryptedKey = keyResponse.SymmetricKeyCipherTextInBytes,
            IsActive = true,
        };
        await _keysRepo.RotateAsync(storageModel, cancellationToken);
    }

    private async Task<byte[]> UnwrapEncryptedKeyAsync(string hsmKeyId, byte[] symmetricEncryptionKey, CancellationToken cancellationToken)
    {
        var unwrapRequest = new UnwrapTenantSymmetricKeyRequest()
        {
            TenantRsaKeyId = hsmKeyId,
            WrappedSymmetricKeyCipherTextInBytes = symmetricEncryptionKey,
        };
        var aesGcmKey = await _hsm.UnrapSymmetricKeyAsync(unwrapRequest, cancellationToken);
        return aesGcmKey.SymmetricKeyPlainTextInBytes;
    }

    private async Task<string> GenerateNewRsaKeyInHsmForTenant(CancellationToken cancellationToken)
    {
        var keyResponse = await _hsm.GenerateTenantIdAsync(cancellationToken);
        return keyResponse.TenantRsaKeyId;
    }

    private async Task<GenerateWrappedSymmetricKeyResponse> GenerateNewAesGcmEncryptionKeyForTenant(string tenantKeyId, CancellationToken cancellationToken)
    {
        var request = new GenerateWrappedSymmetricKeyRequest()
        {
            TenantRsaKeyId = tenantKeyId,
            SymmetricKeyLengthInBytes = SymmetricAesGcmKeyLengthInBytes,
        };
        var keyResponse = await _hsm.GenerateWrappedSymmetricKeyAsync(request, cancellationToken);
        return keyResponse;
    }

    private async Task SaveNewEncryptedSymmetricEncryptionKey(Guid tenantId, string hsmKeyId, GenerateWrappedSymmetricKeyResponse symmetricKey, CancellationToken cancellationToken)
    {
        var storageModel = new PersistenceTenancyKeyModel()
        {
            TenantId = tenantId,
            HsmKeyId = hsmKeyId,
            AesGcmEncryptedKey = symmetricKey.SymmetricKeyCipherTextInBytes,
            IsActive = true,
        };
        await _keysRepo.AddAsync(storageModel, cancellationToken);
    }
}