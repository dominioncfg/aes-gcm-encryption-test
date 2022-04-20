using AesGcmTest.Domain;

namespace AesGcmTest.FunctionalTests.Shared;

public class UserBuilder
{
    private Guid tenantId;
    private Guid id;
    private string email = string.Empty;
    private string firstName = string.Empty;
    private string lastName = string.Empty;


    public UserBuilder WithTenantId(Guid tenantId)
    {
        this.tenantId = tenantId;
        return this;
    }

    public UserBuilder WithId(Guid id)
    {
        this.id = id;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        this.email = email;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        this.firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        this.lastName = lastName;
        return this;
    }

    public User Build()
    {
        return User.Create(tenantId, id, email, firstName, lastName);
    }
}
