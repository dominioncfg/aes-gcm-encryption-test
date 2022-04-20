using Microsoft.Extensions.DependencyInjection;

namespace AesGcmTest.FunctionalTests.Seedwork;

public static class TestServerFixtureExtensions
{
    public static async Task<T> ExecuteScopeAsync<T>(this TestServerFixture fixture, Func<IServiceProvider, Task<T>> function)
    {
        var scopeFactory = fixture.Server.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        return await function(scope.ServiceProvider);
    }

    public static async Task ExecuteScopeAsync(this TestServerFixture fixture, Func<IServiceProvider, Task> function)
    {
        var scopeFactory = fixture.Server.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        await function(scope.ServiceProvider);
    }
}
