using AesGcmTest.Application;
using AesGcmTest.Infrastructure;
using Amazon.KeyManagementService;
using Amazon.Runtime;

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
            .AddSingleton(_=>
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
}
