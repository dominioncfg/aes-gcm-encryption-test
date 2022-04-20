using AesGcmTest.Domain;
using AesGcmTest.FunctionalTests.Seedwork;
using Microsoft.Extensions.DependencyInjection;

namespace AesGcmTest.FunctionalTests.Shared;

internal static class UserExtensions
{
    public static async Task<IEnumerable<User>> GetAllUsersByTenantIdAsync(this TestServerFixture fixture, Guid tenantId)
    {
        return await fixture.ExecuteScopeAsync(async services =>
        {
            var searchRepo = services.GetRequiredService<IUserRepository>();
            return await searchRepo.GetAllByTenantIdAsync(tenantId, default);
        });
    }

    public static async Task AssumeUserInRepositoryAsync(this TestServerFixture fixture, User user)
    {
        await fixture.ExecuteScopeAsync(async services =>
        {
            var searchRepo = services.GetRequiredService<IUserRepository>();
            await searchRepo.AddAsync(user, default);
        });
    }
}
