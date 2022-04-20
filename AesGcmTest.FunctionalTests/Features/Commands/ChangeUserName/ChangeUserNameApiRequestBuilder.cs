using AesGcmTest.Web.Presentation;

namespace AesGcmTest.FunctionalTests.Features;

public class ChangeUserNameApiRequestBuilder
{
    private string firstName = string.Empty;
    private string lastName = string.Empty;


    public ChangeUserNameApiRequestBuilder WithFirstName(string firstName)
    {
        this.firstName = firstName;
        return this;
    }

    public ChangeUserNameApiRequestBuilder WithLastName(string lastName)
    {
        this.lastName = lastName;
        return this;
    }

    public ChangeUserNameApiRequest Build()
    {
        return new ChangeUserNameApiRequest()
        {
            FirstName = firstName,
            LastName = lastName,
        };
    }
}
