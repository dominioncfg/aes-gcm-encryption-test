namespace AesGcmTest.Infrastructure;

public interface ITenancyKeyHardwareSecurityModuleService
{
    public Task<GenerateTenantRsaKeyResponse> GenerateTenantIdAsync(CancellationToken cancellationToken);
    public Task<GenerateWrappedSymmetricKeyResponse> GenerateWrappedSymmetricKeyAsync(GenerateWrappedSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken);
    public Task<UnwrapTenantSymmetricKeyResponse> UnrapSymmetricKeyAsync(UnwrapTenantSymmetricKeyRequest unwrapSymmetricKeyRequest, CancellationToken cancellationToken);
}
