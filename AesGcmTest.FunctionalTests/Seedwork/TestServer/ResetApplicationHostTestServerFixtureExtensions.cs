using AesGcmTest.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AesGcmTest.FunctionalTests.Seedwork;

public static class ResetApplicationHostTestServerFixtureExtensions
{
    public static async Task OnTestInitClearHsm(this TestServerFixture given)
    {
        await given.ExecuteScopeAsync(services =>
        {
            var storage = services.GetRequiredService<Dictionary<string, AsymmetricEncryptionKeyPairResult>>();
            storage.Clear();
            return Task.CompletedTask;
        });
    }

    public static async Task OnTestInitClearTenantEncryptionKeys(this TestServerFixture given)
    {
        await given.ExecuteScopeAsync(services =>
        {
            var storage = services.GetRequiredService<List<PersistenceTenancyKeyModel>>();
            storage.Clear();
            return Task.CompletedTask;
        });
    }

    public static async Task OnTestInitClearUsers(this TestServerFixture given)
    {
        await given.ExecuteScopeAsync(services =>
        {
            var storage = services.GetRequiredService<List<UserEncryptedPersistenceModel>>();
            storage.Clear();
            return Task.CompletedTask;
        });
    }

}