namespace AesGcmTest.Domain;

public class User
{
    public Guid TenantId { get; }
    public Guid Id { get; }    
    public string Email { get; set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    private User(Guid tenantId, Guid id, string email, string firstName, string lastName)
    {
        TenantId = tenantId;
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public static User Create(Guid tenantId,Guid id, string email, string firstName, string lastName) => new(tenantId,id, email, firstName, lastName);
    public void ChangeName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}




