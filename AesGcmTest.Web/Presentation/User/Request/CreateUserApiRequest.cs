namespace AesGcmTest.Web.Presentation;

public record CreateUserApiRequest
{
    public Guid TenantId { get; init; }
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
