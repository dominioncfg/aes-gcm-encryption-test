namespace AesGcmTest.Infrastructure;

public interface ITenancyKeyHardwareSecurityModuleService
{
    public Task<GenerateTenantRsaKeyResponse> GenerateTenantIdAsync(CancellationToken cancellationToken);
    public Task<GenerateWrappedSymmetricKeyResponse> GenerateWrappedSymmetricKeyAsync(GenerateWrappedSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken);
    public Task<WrapSymmetricKeyResponse> WrapSymmetricKeyAsync(WrapSymmetricKeyRequest wrapSymmetricKeyRequest, CancellationToken cancellationToken);
    public Task<UnwrapTenantSymmetricKeyResponse> UnwrapSymmetricKeyAsync(UnwrapTenantSymmetricKeyRequest unwrapSymmetricKeyRequest, CancellationToken cancellationToken);
    public Task RemoveTenantKeyAsync(RemoveTenantIdRequest removeKeyRequest, CancellationToken cancellationToken);
}
