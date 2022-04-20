namespace AesGcmTest.Web.Presentation;

public record GetAllUsersByTenantIdApiQueryResponse
{
    public IEnumerable<GetAllUsersByTenantIdUserApiQueryResponse> Users { get; init; } = Array.Empty<GetAllUsersByTenantIdUserApiQueryResponse>();
}

public record GetAllUsersByTenantIdUserApiQueryResponse
{
    public Guid TenantId { get; init; }
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
