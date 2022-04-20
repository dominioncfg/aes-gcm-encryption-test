using AesGcmTest.Web.Presentation;

namespace AesGcmTest.FunctionalTests.Features;

public class CreateUserApiRequestBuilder
{
    private Guid tenantId;
    private Guid id;
    private string email = string.Empty;
    private string firstName = string.Empty;
    private string lastName = string.Empty;


    public CreateUserApiRequestBuilder WithTenantId(Guid tenantId)
    {
        this.tenantId = tenantId;
        return this;
    }

    public CreateUserApiRequestBuilder WithId(Guid id)
    {
        this.id = id;
        return this;
    }

    public CreateUserApiRequestBuilder WithEmail(string email)
    {
        this.email = email;
        return this;
    }

    public CreateUserApiRequestBuilder WithFirstName(string firstName)
    {
        this.firstName = firstName;
        return this;
    }

    public CreateUserApiRequestBuilder WithLastName(string lastName)
    {
        this.lastName = lastName;
        return this;
    }

    public CreateUserApiRequest Build()
    {
        return new CreateUserApiRequest()
        {
            TenantId = tenantId,
            Id = id,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
        };
    }

}
