namespace AesGcmTest.Web.Presentation;

public record ChangeUserNameApiRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
