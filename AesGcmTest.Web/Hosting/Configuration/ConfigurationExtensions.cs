using AesGcmTest.Application;
using AesGcmTest.Domain;
using AesGcmTest.Infrastructure;
using Amazon.KeyManagementService;
using Amazon.Runtime;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AesGcmTest.Web.Hosting;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommonServices(configuration);
        //return services.AddCompletelyInMemoryTenancyEncryptionStorage();
        return services.AddWithAwsHsmTenancyEncryptionStorage(configuration);
        //return services.AddWithAzureKeyVaultHsmTenancyEncryptionStorage(configuration);

    }

    public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
    {
        var serviceFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

        using var serviceScope = serviceFactory.CreateScope();
        MigrateEncryptedDatabaseAsync(serviceScope).GetAwaiter().GetResult();
        return app;
    }

    private static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddEncryptedDbContext(configuration)
            .AddTransient<IUserRepository, EncryptedUserRepository>()
            .AddTransient<ITenancySymmetricKeyRepository, TenancySymmetricKeyRepository>()
            .AddTransient<ITenancySymmetricKeyService, TenancySymmetricKeyService>()
            .AddTransient<IAuthenticatedEncryptionService, AuthenticatedEncryptionService>();
    }

    private static IServiceCollection AddEncryptedDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EncrytedDb");
        IServiceCollection serviceCollection = services.AddDbContext<AesGcmDbContext>(options =>
            options
                .UseNpgsql(connectionString)
            );
        return services;
    }

    private static async Task MigrateEncryptedDatabaseAsync(IServiceScope serviceScope)
    {
        await serviceScope.ServiceProvider.GetRequiredService<AesGcmDbContext>().Database.MigrateAsync();
    }
   
    private static IServiceCollection AddCompletelyInMemoryTenancyEncryptionStorage(this IServiceCollection services)
    {
        return services
            .AddTransient<IRsaKeyLocalRepository, RsaKeyLocalRepository>()
            .AddTransient<ITenancyKeyHardwareSecurityModuleService, LocalTenancyKeyHardwareSecurityModuleService>();
    }

    private static IServiceCollection AddWithAwsHsmTenancyEncryptionStorage(this IServiceCollection services, IConfiguration configuration)
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
            .AddTransient<ITenancyKeyHardwareSecurityModuleService, AwsKmsTenancyKeyHardwareSecurityModuleService>();
    }

    private static IServiceCollection AddWithAzureKeyVaultHsmTenancyEncryptionStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureKeyVaultKeyClientConfiguration>(configuration.GetSection(AzureKeyVaultKeyClientConfiguration.Section));
        return services
            .AddSingleton(sp =>
            {
                var azureOptions = sp.GetRequiredService<IOptions<AzureKeyVaultKeyClientConfiguration>>().Value;
                var credentials = new ClientSecretCredential(azureOptions.TenantId, azureOptions.ClientId, azureOptions.ClientSecret);
                return new KeyClient(new Uri(azureOptions.VaultUrl), credentials);
            })
            .AddTransient<ITenancyKeyHardwareSecurityModuleService, AzureKeyVaultTenancyKeyHardwareSecurityModuleService>();            
    }
}
