using AesGcmTest.Domain;
using AesGcmTest.FunctionalTests.Seedwork;
using AesGcmTest.FunctionalTests.Shared;
using AesGcmTest.Web.Presentation;
using FluentAssertions;
using Xunit;

namespace AesGcmTest.FunctionalTests.Features;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenCreatingUsers
{
    private readonly Guid Id = Guid.NewGuid();
    private readonly TestServerFixture Given;

    public WhenCreatingUsers(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task CanCreateSingleUser()
    {
        var request = new CreateUserApiRequestBuilder()
            .WithTenantId(Guid.NewGuid())
            .WithId(Guid.NewGuid())
            .WithFirstName("Perico")
            .WithLastName("Perez")
            .Build();

        await PostAndCreateAsync(request);

        var tenantUsers = await Given.GetAllUsersByTenantIdAsync(request.TenantId);
        tenantUsers.Should().HaveCount(1);

        AssertUserFromRequest(tenantUsers.First(), request);
    }

    private static void AssertUserFromRequest(User actual, CreateUserApiRequest expected)
    {
        actual.TenantId.Should().Be(expected.TenantId);
        actual.Id.Should().Be(expected.Id);
        actual.Email.Should().Be(expected.Email);
        actual.FirstName.Should().Be(expected.FirstName);
        actual.LastName.Should().Be(expected.LastName);
    }

    public async Task PostAndCreateAsync(CreateUserApiRequest request)
    {
        var url = PostCreateUserUrl();
        await Given.Server.CreateClient().PostAndExpectCreatedAsync(url, request);
    }

    private static string PostCreateUserUrl() => $"api/v1/users/";
}
