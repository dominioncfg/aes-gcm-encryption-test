using AesGcmTest.Domain;
using AesGcmTest.FunctionalTests.Seedwork;
using AesGcmTest.FunctionalTests.Shared;
using AesGcmTest.Web.Presentation;
using FluentAssertions;
using Xunit;

namespace AesGcmTest.FunctionalTests.Features;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenRotatingTenantKey
{
    private readonly Guid Id = Guid.NewGuid();
    private readonly TestServerFixture Given;

    public WhenRotatingTenantKey(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task AfterRotatingTenantKeyStillAllUsersCanBeObtained()
    {
        var existingUser = new UserBuilder()
            .WithTenantId(Guid.NewGuid())
            .WithId(Guid.NewGuid())
            .WithEmail("pericoperez@gmail.com")
            .WithFirstName("Perico")
            .WithLastName("Perez")
            .Build();
        await Given.AssumeUserInRepositoryAsync(existingUser);

        await RotateTenantKeyAsync(existingUser.TenantId);

        var response = await GetApiUserByIdAsync(existingUser.Id);
        var tenantUsers = await Given.GetAllUsersByTenantIdAsync(existingUser.TenantId);
        tenantUsers.Should().HaveCount(1);

        AssertResponseFromUser(response, existingUser);
    }

    private static void AssertResponseFromUser(GetUserByIdQueryResponse actual, User expected)
    {
        actual.TenantId.Should().Be(expected.TenantId);
        actual.Id.Should().Be(expected.Id);
        actual.Email.Should().Be(expected.Email);
        actual.FirstName.Should().Be(expected.FirstName);
        actual.LastName.Should().Be(expected.LastName);
    }

    public async Task<GetUserByIdQueryResponse> GetApiUserByIdAsync(Guid userId)
    {
        var url = GetUserByIdUrl(userId);
        return await Given.Server.CreateClient().GetAsync<GetUserByIdQueryResponse>(url);
    }

    public async Task RotateTenantKeyAsync(Guid tenantId)
    {
        var url = RotateTenantKeyUrl(tenantId);
        await Given.Server.CreateClient().PutAndExpectNoContentAsync(url);
    }

    private static string RotateTenantKeyUrl(Guid tenantId) => $"api/v1/tenants/{tenantId}/rotate-key";
    private static string GetUserByIdUrl(Guid id) => $"api/v1/users/{id}";
}
