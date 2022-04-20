namespace AesGcmTest.Infrastructure;
public class InMemoryTenancyKeyHardwareSecurityModuleService : ITenancyKeyHardwareSecurityModuleService
{
    private const int RsaKeySize = 4096;
    private readonly Dictionary<string, AsymmetricEncryptionKeyPairResult> _hsmKeyStorage;

    public InMemoryTenancyKeyHardwareSecurityModuleService(Dictionary<string, AsymmetricEncryptionKeyPairResult> hsmKeyStorage)
    {
        _hsmKeyStorage = hsmKeyStorage;
    }

    public Task<GenerateTenantRsaKeyResponse> GenerateTenantIdAsync(CancellationToken cancellationToken)
    {
        var keyId = GenerateRandomStorageKey();
        var keyResult = RsaAsymmetricEncryption.CreateKeyPair(RsaKeySize);

        _hsmKeyStorage.Add(keyId, keyResult);

        return Task.FromResult(new GenerateTenantRsaKeyResponse()
        {
            TenantRsaKeyId = keyId,
        });
    }

    public Task<GenerateWrappedSymmetricKeyResponse> GenerateWrappedSymmetricKeyAsync(GenerateWrappedSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        MakeSureKeyIsExists(wrapSymmetricKeyRequest.TenantRsaKeyId);
        var rsaKey = _hsmKeyStorage[wrapSymmetricKeyRequest.TenantRsaKeyId];

        var symmetricKey = CryptographicKey.CreateRandomOfBytes(wrapSymmetricKeyRequest.SymmetricKeyLengthInBytes);

        var encryptedSymmeticKey = RsaAsymmetricEncryption.Encrypt(symmetricKey.Bytes, rsaKey.PublicKeyInBytes);

        return Task.FromResult(new GenerateWrappedSymmetricKeyResponse()
        {
            SymmetricKeyPlainTextInBytes = symmetricKey.Bytes,
            SymmetricKeyCipherTextInBytes = encryptedSymmeticKey.ChipherTextInBytes,
        });
    }

    public Task<WrapSymmetricKeyResponse> WrapSymmetricKeyAsync(WrapSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        MakeSureKeyIsExists(wrapSymmetricKeyRequest.TenantRsaKeyId);
        var rsaKey = _hsmKeyStorage[wrapSymmetricKeyRequest.TenantRsaKeyId];
        var encryptedSymmeticKey = RsaAsymmetricEncryption.Encrypt(wrapSymmetricKeyRequest.SymmetricKey, rsaKey.PublicKeyInBytes);

        return Task.FromResult(new WrapSymmetricKeyResponse()
        {
            SymmetricKeyCipherTextInBytes = encryptedSymmeticKey.ChipherTextInBytes,
        });
    }

    public Task<UnwrapTenantSymmetricKeyResponse> UnrapSymmetricKeyAsync(UnwrapTenantSymmetricKeyRequest unwrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        MakeSureKeyIsExists(unwrapSymmetricKeyRequest.TenantRsaKeyId);
        var rsaKey = _hsmKeyStorage[unwrapSymmetricKeyRequest.TenantRsaKeyId];

        var decryptedSymmeticKey = RsaAsymmetricEncryption.Decrypt(unwrapSymmetricKeyRequest.WrappedSymmetricKeyCipherTextInBytes, rsaKey.PrivateKeyInBytes);

        return Task.FromResult(new UnwrapTenantSymmetricKeyResponse()
        {
            SymmetricKeyPlainTextInBytes = decryptedSymmeticKey.PlainTextInBytes,
        });
    }


    private void MakeSureKeyIsExists(string keyId)
    {
        if (!_hsmKeyStorage.ContainsKey(keyId))
            throw new Exception("The key don't exist :(.");
    }

    private static string GenerateRandomStorageKey() => $"urn:enc_companty:hsm_key:{Guid.NewGuid()}";
}
