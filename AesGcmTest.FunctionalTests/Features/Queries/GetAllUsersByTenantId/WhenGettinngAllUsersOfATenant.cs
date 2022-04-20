using AesGcmTest.Domain;
using AesGcmTest.FunctionalTests.Seedwork;
using AesGcmTest.FunctionalTests.Shared;
using AesGcmTest.Web.Presentation;
using FluentAssertions;
using Xunit;

namespace AesGcmTest.FunctionalTests.Features;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenGettinngAllUsersOfATenant
{
    private readonly Guid Id = Guid.NewGuid();
    private readonly TestServerFixture Given;

    public WhenGettinngAllUsersOfATenant(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task CanGetTenantWithSingleUser()
    {
        var existingUser = new UserBuilder()
            .WithTenantId(Guid.NewGuid())
            .WithId(Guid.NewGuid())
            .WithEmail("pericoperez@gmail.com")
            .WithFirstName("Perico")
            .WithLastName("Perez")
            .Build();
        await Given.AssumeUserInRepositoryAsync(existingUser);

        var response = await GetTenantUsersAsync(existingUser.TenantId);
        response.Users.Should().NotBeNull().And.HaveCount(1);

        var tenantUsers = await Given.GetAllUsersByTenantIdAsync(existingUser.TenantId);
        tenantUsers.Should().HaveCount(1);

        AssertResponseFromUser(response.Users.First(), existingUser);
    }

    [Fact]
    [ResetApplicationState]
    public async Task CanGetTenantWithTwoUsers()
    {
        var tenantId = Guid.NewGuid();
        var existingFirstUser = new UserBuilder()
            .WithTenantId(tenantId)
            .WithId(Guid.NewGuid())
            .WithEmail("pericoperez@gmail.com")
            .WithFirstName("Perico")
            .WithLastName("Perez")
            .Build();
        var existingSecondUser = new UserBuilder()
          .WithTenantId(tenantId)
          .WithId(Guid.NewGuid())
          .WithEmail("dabadabadu@gmail.com")
          .WithFirstName("Pedro")
          .WithLastName("Picapiedra")
          .Build();
        await Given.AssumeUserInRepositoryAsync(existingFirstUser);
        await Given.AssumeUserInRepositoryAsync(existingSecondUser);

        var response = await GetTenantUsersAsync(tenantId);
        response.Users.Should().NotBeNull().And.HaveCount(2);

        var tenantUsers = await Given.GetAllUsersByTenantIdAsync(tenantId);
        tenantUsers.Should().HaveCount(2);

        AssertResponseFromUser(response.Users.First(x => x.Id == existingFirstUser.Id), existingFirstUser);
        AssertResponseFromUser(response.Users.First(x => x.Id == existingSecondUser.Id), existingSecondUser);
    }

    [Fact]
    [ResetApplicationState]
    public async Task DoesNotReturnsAnotherTenantUser()
    {
        var tenantId = Guid.NewGuid();
        var invalidTenantId = Guid.NewGuid();
        var existingFirstUser = new UserBuilder()
            .WithTenantId(tenantId)
            .WithId(Guid.NewGuid())
            .WithEmail("pericoperez@gmail.com")
            .WithFirstName("Perico")
            .WithLastName("Perez")
            .Build();
        var anotherTenantUser = new UserBuilder()
          .WithTenantId(invalidTenantId)
          .WithId(Guid.NewGuid())
          .WithEmail("dabadabadu@gmail.com")
          .WithFirstName("Pedro")
          .WithLastName("Picapiedra")
          .Build();
        await Given.AssumeUserInRepositoryAsync(existingFirstUser);
        await Given.AssumeUserInRepositoryAsync(anotherTenantUser);

        var response = await GetTenantUsersAsync(tenantId);
        response.Users.Should().NotBeNull().And.HaveCount(1);

        var tenantUsers = await Given.GetAllUsersByTenantIdAsync(tenantId);
        tenantUsers.Should().HaveCount(1);

        AssertResponseFromUser(response.Users.First(x => x.Id == existingFirstUser.Id), existingFirstUser);
    }

    private static void AssertResponseFromUser(GetAllUsersByTenantIdUserApiQueryResponse actual, User expected)
    {
        actual.TenantId.Should().Be(expected.TenantId);
        actual.Id.Should().Be(expected.Id);
        actual.Email.Should().Be(expected.Email);
        actual.FirstName.Should().Be(expected.FirstName);
        actual.LastName.Should().Be(expected.LastName);
    }

    public async Task<GetAllUsersByTenantIdApiQueryResponse> GetTenantUsersAsync(Guid tenantId)
    {
        var url = GetTenantUsersUrl(tenantId);
        return await Given.Server.CreateClient().GetAsync<GetAllUsersByTenantIdApiQueryResponse>(url);
    }

    private static string GetTenantUsersUrl(Guid id) => $"api/v1/tenants/{id}/users";
}
