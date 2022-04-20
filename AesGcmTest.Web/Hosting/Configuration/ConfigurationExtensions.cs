using AesGcmTest.Application;
using AesGcmTest.Infrastructure;

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
}
