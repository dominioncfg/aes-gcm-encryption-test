using AesGcmTest.Domain;


namespace AesGcmTest.Infrastructure;

public class UserPersistenceModel
{
    public Guid TenantId { get; set; }
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    private UserPersistenceModel(Guid tenantId, Guid id, string email, string firstName, string lastName)
    {
        TenantId = tenantId;
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public static UserPersistenceModel FromDomainUser(User user) => new(user.TenantId, user.Id, user.Email, user.FirstName, user.LastName);

    public User ConvertToDomain() => User.Create(TenantId, Id, Email, FirstName, LastName);
}

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<UserPersistenceModel> _usersStorage;

    public InMemoryUserRepository(List<UserPersistenceModel> usersStorage)
    {
        _usersStorage = usersStorage;
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _usersStorage.Add(UserPersistenceModel.FromDomainUser(user));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var persistenceUserIndex = _usersStorage.FindIndex(x => x.Id == user.Id); 
        if(persistenceUserIndex == -1) 
            throw new Exception("User not found");

        _usersStorage[persistenceUserIndex] = UserPersistenceModel.FromDomainUser(user);

        return Task.CompletedTask;
    }

    public Task<User> GetByIdAsync(Guid id, CancellationToken cancellation)
    {
        var user = _usersStorage.FirstOrDefault(x => x.Id == id) ?? throw new Exception("User not found");
        return Task.FromResult(user.ConvertToDomain());
    }

    public Task<IEnumerable<User>> GetAllByTenantIdAsync(Guid tenantId, CancellationToken cancellation)
    {
        var tenantUsers = _usersStorage
            .Where(x => x.TenantId == tenantId)
            .Select(x => x.ConvertToDomain())
            .ToList();
        return Task.FromResult<IEnumerable<User>>(tenantUsers);
    }
}