using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Options;

namespace AesGcmTest.Infrastructure;
public class AzureKeyVaultTenancyKeyHardwareSecurityModuleService : ITenancyKeyHardwareSecurityModuleService
{
    private readonly KeyClient _azureKeyClient;
    private readonly AzureKeyVaultKeyClientConfiguration _azureKvOptions;

    public AzureKeyVaultTenancyKeyHardwareSecurityModuleService(KeyClient azureKeyClient, IOptions<AzureKeyVaultKeyClientConfiguration> azureKvOptions)
    {
        _azureKeyClient = azureKeyClient;
        _azureKvOptions = azureKvOptions.Value;
    }

    public async Task<GenerateTenantRsaKeyResponse> GenerateTenantIdAsync(CancellationToken cancellationToken)
    {
        var keyName = GenerateRandomStorageKey();

        var rsaKey = new CreateRsaKeyOptions(keyName, hardwareProtected: false)
        {
            KeySize = 4096,
        };
        var azureResponse = await _azureKeyClient.CreateRsaKeyAsync(rsaKey, cancellationToken);


        return new GenerateTenantRsaKeyResponse()
        {
            TenantRsaKeyId = azureResponse.Value.Id.ToString(),
        };
    }

    public async Task<GenerateWrappedSymmetricKeyResponse> GenerateWrappedSymmetricKeyAsync(GenerateWrappedSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        var symmetricKey = CryptographicKey.CreateRandomOfBytes(wrapSymmetricKeyRequest.SymmetricKeyLengthInBytes);

        var cryptoKeyClient = GetKeyCryptographyClient(wrapSymmetricKeyRequest.TenantRsaKeyId);
        var azureResponse = await cryptoKeyClient.WrapKeyAsync(KeyWrapAlgorithm.RsaOaep, symmetricKey.Bytes, cancellationToken);

        return new GenerateWrappedSymmetricKeyResponse()
        {
            SymmetricKeyPlainTextInBytes = symmetricKey.Bytes,
            SymmetricKeyCipherTextInBytes = azureResponse.EncryptedKey,
        };
    }

    public async Task<WrapSymmetricKeyResponse> WrapSymmetricKeyAsync(WrapSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        var cryptoKeyClient = GetKeyCryptographyClient(wrapSymmetricKeyRequest.TenantRsaKeyId);
        var response = await cryptoKeyClient.WrapKeyAsync(KeyWrapAlgorithm.RsaOaep, wrapSymmetricKeyRequest.SymmetricKey, cancellationToken);

        return new WrapSymmetricKeyResponse()
        {
            SymmetricKeyCipherTextInBytes = response.EncryptedKey,
        };
    }

    public async Task<UnwrapTenantSymmetricKeyResponse> UnwrapSymmetricKeyAsync(UnwrapTenantSymmetricKeyRequest unwrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        var cryptoKeyClient = GetKeyCryptographyClient(unwrapSymmetricKeyRequest.TenantRsaKeyId);
        var response = await cryptoKeyClient.UnwrapKeyAsync(KeyWrapAlgorithm.RsaOaep, unwrapSymmetricKeyRequest.WrappedSymmetricKeyCipherTextInBytes, cancellationToken);

        return new UnwrapTenantSymmetricKeyResponse()
        {
            SymmetricKeyPlainTextInBytes = response.Key,
        };
    }

    private static string GenerateRandomStorageKey() => $"urn--enc-companty--hsm--{Guid.NewGuid()}";

    private CryptographyClient GetKeyCryptographyClient(string keyId)
    {
        var credentials = new ClientSecretCredential(_azureKvOptions.TenantId, _azureKvOptions.ClientId, _azureKvOptions.ClientSecret);
        var cryptoKeyClient = new CryptographyClient(new Uri(keyId), credentials);
        return cryptoKeyClient;
    }
}
