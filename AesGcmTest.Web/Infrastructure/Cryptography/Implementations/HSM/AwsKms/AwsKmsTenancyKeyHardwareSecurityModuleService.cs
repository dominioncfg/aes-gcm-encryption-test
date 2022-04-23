using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;

namespace AesGcmTest.Infrastructure;

public class AwsKmsTenancyKeyHardwareSecurityModuleService : ITenancyKeyHardwareSecurityModuleService
{
    private readonly AmazonKeyManagementServiceClient _keyManagementServiceClient;

    public AwsKmsTenancyKeyHardwareSecurityModuleService(AmazonKeyManagementServiceClient keyManagementServiceClient)
    {
        _keyManagementServiceClient = keyManagementServiceClient;
    }

    public async Task<GenerateTenantRsaKeyResponse> GenerateTenantIdAsync(CancellationToken cancellationToken)
    {
        var awsRequest = new CreateKeyRequest()
        {
            KeySpec = CustomerMasterKeySpec.RSA_4096.Value,
            KeyUsage = KeyUsageType.ENCRYPT_DECRYPT
        };
        var awsRsaKeyResponse = await _keyManagementServiceClient.CreateKeyAsync(awsRequest, cancellationToken);

        KeyMetadata keyMetadata = awsRsaKeyResponse.KeyMetadata;

        return new GenerateTenantRsaKeyResponse()
        {
            TenantRsaKeyId = keyMetadata.Arn,
        };
    }

    public async Task<GenerateWrappedSymmetricKeyResponse> GenerateWrappedSymmetricKeyAsync(GenerateWrappedSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        var symmetricKey = CryptographicKey.CreateRandomOfBytes(wrapSymmetricKeyRequest.SymmetricKeyLengthInBytes);
        using var keyStream = new MemoryStream(symmetricKey.Bytes);

        var awsRequest = new EncryptRequest()
        {
            KeyId = wrapSymmetricKeyRequest.TenantRsaKeyId,
            Plaintext = keyStream,

        };
        var awsResponse = await _keyManagementServiceClient.EncryptAsync(awsRequest, cancellationToken);

        var encryptedKey = awsResponse.CiphertextBlob.ToArray();
        return new GenerateWrappedSymmetricKeyResponse()
        {
            SymmetricKeyPlainTextInBytes = symmetricKey.Bytes,
            SymmetricKeyCipherTextInBytes = encryptedKey,
        };
    }

    public async Task<WrapSymmetricKeyResponse> WrapSymmetricKeyAsync(WrapSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        using var keyStream = new MemoryStream(wrapSymmetricKeyRequest.SymmetricKey);

        var awsRequest = new EncryptRequest()
        {
            KeyId = wrapSymmetricKeyRequest.TenantRsaKeyId,
            Plaintext = keyStream,
        };
        var awsResponse = await _keyManagementServiceClient.EncryptAsync(awsRequest, cancellationToken);
        
        var encryptedKey = awsResponse.CiphertextBlob.ToArray();
        return new WrapSymmetricKeyResponse()
        {
            SymmetricKeyCipherTextInBytes = encryptedKey,
        };
    }

    public async Task<UnwrapTenantSymmetricKeyResponse> UnwrapSymmetricKeyAsync(UnwrapTenantSymmetricKeyRequest unwrapSymmetricKeyRequest, CancellationToken cancellationToken)
    {
        using var keyStream = new MemoryStream(unwrapSymmetricKeyRequest.WrappedSymmetricKeyCipherTextInBytes);
        
        var awsRequest = new DecryptRequest()
        {
            KeyId = unwrapSymmetricKeyRequest.TenantRsaKeyId,
            CiphertextBlob = keyStream,
        };
        var awsResponse = await _keyManagementServiceClient.DecryptAsync(awsRequest, cancellationToken);


        return new UnwrapTenantSymmetricKeyResponse()
        {
            SymmetricKeyPlainTextInBytes = awsResponse.Plaintext.ToArray(),
        };
    }

    public Task RemoveTenantKeyAsync(RemoveTenantIdRequest removeKeyRequest, CancellationToken cancellationToken)
    {
        //Local Stack AWS returns error :(
        //var awsRequest = new ScheduleKeyDeletionRequest()
        //{
        //    KeyId = removeKeyRequest.TenantRsaKeyId,
        //    PendingWindowInDays = 7,
        //};
        //await _keyManagementServiceClient.ScheduleKeyDeletionAsync(awsRequest, cancellationToken);
        return Task.CompletedTask;
    }
}
