using AesGcmTest.Domain;

namespace AesGcmTest.Infrastructure;

public class UserPersistenceDto
{
    public Guid TenantId { get; }
    public Guid Id { get; }
    public string Email { get; set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public UserPersistenceDto(Guid tenantId, Guid id, string email, string firstName, string lastName)
    {
        TenantId = tenantId;
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public static UserPersistenceDto FromDomain(User user) => new (user.TenantId, user.Id, user.Email, user.FirstName, user.LastName);

    public User ToDomain() => User.Create(TenantId, Id, Email, FirstName, LastName);
}


