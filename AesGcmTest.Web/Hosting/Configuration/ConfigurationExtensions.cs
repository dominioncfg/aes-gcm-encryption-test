using AesGcmTest.Application;
using AesGcmTest.Infrastructure;
using Amazon.KeyManagementService;
using Amazon.Runtime;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Microsoft.Extensions.Options;

namespace AesGcmTest.Web.Hosting;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddCompletelyInMemoryTenancyEncryptionStorage(this IServiceCollection services)
    {
        return services
            .AddSingleton(new List<UserEncryptedPersistenceModel>())
            .AddSingleton(new Dictionary<string, AsymmetricEncryptionKeyPairResult>())
            .AddSingleton(new List<PersistenceTenancyKeyModel>())

            .AddTransient<ITenancyKeyHardwareSecurityModuleService, InMemoryTenancyKeyHardwareSecurityModuleService>()
            .AddTransient<ITenancySymmetricKeyRepository, InMemoryTenancySymmetricKeyRepository>()
            .AddTransient<ITenancySymmetricKeyService, TenancySymmetricKeyService>()
            .AddTransient<IAuthenticatedEncryptionService, AuthenticatedEncryptionService>();
    }

    public static IServiceCollection AddWithAwsHsmTenancyEncryptionStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var awsOptions = new AwsKeyManagementClientConfiguration();
        configuration.GetSection(AwsKeyManagementClientConfiguration.Section).Bind(awsOptions);

        return services
            .AddSingleton(_ =>
            {
                var creadentials = new BasicAWSCredentials(awsOptions.AccessKey, awsOptions.SecretKey);
                var config = new AmazonKeyManagementServiceConfig()
                {
                    UseHttp = awsOptions.UseHttp,
                    AuthenticationRegion = awsOptions.RegionName,
                    ServiceURL = awsOptions.ServiceUrl,
                };
                return new AmazonKeyManagementServiceClient(creadentials, config);
            })
            .AddSingleton(new List<UserEncryptedPersistenceModel>())
            .AddSingleton(new List<PersistenceTenancyKeyModel>())
            .AddTransient<ITenancyKeyHardwareSecurityModuleService, AwsKmsTenancyKeyHardwareSecurityModuleService>()
            .AddTransient<ITenancySymmetricKeyRepository, InMemoryTenancySymmetricKeyRepository>()
            .AddTransient<ITenancySymmetricKeyService, TenancySymmetricKeyService>()
            .AddTransient<IAuthenticatedEncryptionService, AuthenticatedEncryptionService>();
    }

    public static IServiceCollection AddWithAzureKeyVaultHsmTenancyEncryptionStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureKeyVaultKeyClientConfiguration>(configuration.GetSection(AzureKeyVaultKeyClientConfiguration.Section));
        return services
            .AddSingleton(sp =>
            {
                var azureOptions = sp.GetRequiredService<IOptions<AzureKeyVaultKeyClientConfiguration>>().Value;
                var credentials = new ClientSecretCredential(azureOptions.TenantId, azureOptions.ClientId, azureOptions.ClientSecret);
                return new KeyClient(new Uri(azureOptions.VaultUrl), credentials);
            })
            .AddSingleton(new List<UserEncryptedPersistenceModel>())
            .AddSingleton(new List<PersistenceTenancyKeyModel>())
            .AddTransient<ITenancyKeyHardwareSecurityModuleService, AzureKeyVaultTenancyKeyHardwareSecurityModuleService>()
            .AddTransient<ITenancySymmetricKeyRepository, InMemoryTenancySymmetricKeyRepository>()
            .AddTransient<ITenancySymmetricKeyService, TenancySymmetricKeyService>()
            .AddTransient<IAuthenticatedEncryptionService, AuthenticatedEncryptionService>();
    }
}
