using AesGcmTest.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AesGcmTest.FunctionalTests.Seedwork;

public static class ResetApplicationHostTestServerFixtureExtensions
{
    public static async Task OnTestInitClearPostgreSqlDb(this TestServerFixture given)
    {
        await given!.ExecuteScopeAsync(async services =>
        {
            var dataStorage = services.GetRequiredService<AesGcmDbContext>();
            await dataStorage.Database.EnsureDeletedAsync();
            await dataStorage.Database.MigrateAsync();
        });
    } 
}