namespace AesGcmTest.Infrastructure;

public class AzureKeyVaultKeyClientConfiguration
{
    public const string Section = "AzureKeyVault";
    public string VaultUrl { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;

}
