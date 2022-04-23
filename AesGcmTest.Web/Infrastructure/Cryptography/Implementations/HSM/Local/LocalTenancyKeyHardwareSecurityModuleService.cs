namespace AesGcmTest.Infrastructure;

public class LocalTenancyKeyHardwareSecurityModuleService : ITenancyKeyHardwareSecurityModuleService
{
    private const int RsaKeySize = 4096;
    private readonly IRsaKeyLocalRepository _keysRepo;

    public LocalTenancyKeyHardwareSecurityModuleService(IRsaKeyLocalRepository keysRepo)
    {
        _keysRepo = keysRepo;
    }

    public async Task<GenerateTenantRsaKeyResponse> GenerateTenantIdAsync(CancellationToken cancellationToken)
    {
        var keyId = GenerateRandomStorageKey();
        var keyResult = RsaAsymmetricEncryption.CreateKeyPair(RsaKeySize);

        var keyModel = PersistenceRsaKeyModel.Create(keyId, keyResult.PrivateKeyInBytes, keyResult.PublicKeyInBytes);
        await _keysRepo.AddAsync(keyModel, cancellationToken);

        return new GenerateTenantRsaKeyResponse()
        {
            TenantRsaKeyId = keyId,
        };
    }

    public async Task<GenerateWrappedSymmetricKeyResponse> GenerateWrappedSymmetricKeyAsync(GenerateWrappedSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        var rsaKey = await _keysRepo.GetByFriendlyIdAsync(wrapSymmetricKeyRequest.TenantRsaKeyId, cancellationToken);

        var symmetricKey = CryptographicKey.CreateRandomOfBytes(wrapSymmetricKeyRequest.SymmetricKeyLengthInBytes);

        var encryptedSymmeticKey = RsaAsymmetricEncryption.Encrypt(symmetricKey.Bytes, rsaKey.PublicKey);

        return new GenerateWrappedSymmetricKeyResponse()
        {
            SymmetricKeyPlainTextInBytes = symmetricKey.Bytes,
            SymmetricKeyCipherTextInBytes = encryptedSymmeticKey.ChipherTextInBytes,
        };
    }

    public async Task<WrapSymmetricKeyResponse> WrapSymmetricKeyAsync(WrapSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        var rsaKey = await _keysRepo.GetByFriendlyIdAsync(wrapSymmetricKeyRequest.TenantRsaKeyId, cancellationToken);
        var encryptedSymmeticKey = RsaAsymmetricEncryption.Encrypt(wrapSymmetricKeyRequest.SymmetricKey, rsaKey.PublicKey);

        return new WrapSymmetricKeyResponse()
        {
            SymmetricKeyCipherTextInBytes = encryptedSymmeticKey.ChipherTextInBytes,
        };
    }

    public async Task<UnwrapTenantSymmetricKeyResponse> UnwrapSymmetricKeyAsync(UnwrapTenantSymmetricKeyRequest unwrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        var rsaKey = await _keysRepo.GetByFriendlyIdAsync(unwrapSymmetricKeyRequest.TenantRsaKeyId, cancellationToken);

        var decryptedSymmeticKey = RsaAsymmetricEncryption.Decrypt(unwrapSymmetricKeyRequest.WrappedSymmetricKeyCipherTextInBytes, rsaKey.PrivateKey);

        return new UnwrapTenantSymmetricKeyResponse()
        {
            SymmetricKeyPlainTextInBytes = decryptedSymmeticKey.PlainTextInBytes,
        };
    }

    private static string GenerateRandomStorageKey() => $"urn:enc_companty:hsm_key:{Guid.NewGuid()}";
}
