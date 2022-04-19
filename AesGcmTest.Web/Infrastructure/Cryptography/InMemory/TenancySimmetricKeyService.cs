namespace AesGcmTest.Infrastructure;

public class TenancySimmetricKeyService : ITenancySimmetricKeyService
{
    private readonly ITenancyKeyHardwareSecurityModuleService _hsm;
    private readonly ITenancySymmetricKeyRepository _keysRepo;
    private const int SymmetricAesGcmKeyLengthInBytes = 32;

    public TenancySimmetricKeyService(ITenancyKeyHardwareSecurityModuleService hsm, ITenancySymmetricKeyRepository keysRepo)
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
        };
        await _keysRepo.Add(storageModel, cancellationToken);
    }
}