namespace AesGcmTest.Domain;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task<User> GetByIdAsync(Guid id, CancellationToken cancellation);
    Task<IEnumerable<User>> GetAllByTenantIdAsync(Guid tenantId, CancellationToken cancellation);
}
