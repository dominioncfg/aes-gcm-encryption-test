using AesGcmTest.Domain;
using AesGcmTest.FunctionalTests.Seedwork;
using AesGcmTest.FunctionalTests.Shared;
using AesGcmTest.Web.Presentation;
using FluentAssertions;
using Xunit;

namespace AesGcmTest.FunctionalTests.Features;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenChangingUserName
{
    private readonly Guid Id = Guid.NewGuid();
    private readonly TestServerFixture Given;

    public WhenChangingUserName(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task CanChangeSingleUserName()
    {
        var existingUser = new UserBuilder()
            .WithTenantId(Guid.NewGuid())
            .WithId(Guid.NewGuid())
            .WithEmail("pericoperez@gmail.com")
            .WithFirstName("WrongFirstName")
            .WithLastName("WrongLastName")
            .Build();
        await Given.AssumeUserInRepositoryAsync(existingUser);

        var request = new ChangeUserNameApiRequestBuilder()
            .WithFirstName("Perico")
            .WithLastName("Perez")
            .Build();

        await PutAndChangeName(existingUser.Id, request);

        var tenantUsers = await Given.GetAllUsersByTenantIdAsync(existingUser.TenantId);
        tenantUsers.Should().HaveCount(1);

        AssertUserFromRequest(tenantUsers.First(), request, existingUser);
    }

    private static void AssertUserFromRequest(User actual, ChangeUserNameApiRequest expected, User expectedOriginal)
    {
        actual.TenantId.Should().Be(expectedOriginal.TenantId);
        actual.Id.Should().Be(expectedOriginal.Id);
        actual.Email.Should().Be(expectedOriginal.Email);
        actual.FirstName.Should().Be(expected.FirstName);
        actual.LastName.Should().Be(expected.LastName);
    }

    public async Task PutAndChangeName(Guid userId, ChangeUserNameApiRequest request)
    {
        var url = PutChangeUserNameCreateUserUrl(userId);
        await Given.Server.CreateClient().PutAndExpectNoContentAsync(url, request);
    }

    private static string PutChangeUserNameCreateUserUrl(Guid userId) => $"api/v1/users/{userId}";
}
