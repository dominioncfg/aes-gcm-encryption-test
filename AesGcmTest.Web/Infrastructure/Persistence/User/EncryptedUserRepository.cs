using AesGcmTest.Application;
using AesGcmTest.Domain;
using Microsoft.EntityFrameworkCore;

namespace AesGcmTest.Infrastructure;

public class EncryptedUserRepository : IUserRepository
{
    private readonly AesGcmDbContext _dbContext;
    private readonly ITenancySymmetricKeyService _tenancySymmetricKeyService;
    private readonly IAuthenticatedEncryptionService _encryptionService;

    public EncryptedUserRepository(AesGcmDbContext dbContext,
        ITenancySymmetricKeyService tenancySimmetricKeyService,
        IAuthenticatedEncryptionService encryptionService)
    {
        _dbContext = dbContext;
        _tenancySymmetricKeyService = tenancySimmetricKeyService;
        _encryptionService = encryptionService;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        var nonce = AesGcmSymmetricEncryption.GetRandomNonce();
        UserEncryptedPersistenceModel encryptedUser = await EncryptUserAsync(user, nonce.Bytes, cancellationToken);

        await _dbContext.AddAsync(encryptedUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var nonce = AesGcmSymmetricEncryption.GetRandomNonce();
        UserEncryptedPersistenceModel encryptedUser = await EncryptUserAsync(user, nonce.Bytes, cancellationToken);

        _dbContext.Update(encryptedUser);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var userEncrypted = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (userEncrypted is null)
            throw new Exception("User not found");

        return await DecryptEncryptedUser(userEncrypted, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenantEncryptedUsers = await _dbContext
            .Users
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        var users = new List<User>();
        foreach (var userEncrypted in tenantEncryptedUsers)
        {
            var user = await DecryptEncryptedUser(userEncrypted, cancellationToken);
            users.Add(user);
        }
        return users;
    }

    private async Task<UserEncryptedPersistenceModel> EncryptUserAsync(User user, byte[] nonce, CancellationToken cancellationToken)
    {
        var userPersistence = UserPersistenceDto.FromDomain(user);
        var symmetricKey = await _tenancySymmetricKeyService.GetOrCreateTenantSymmetricEncryptionKeyAsync(user.TenantId, cancellationToken);
        var encryptionRequest = new AuthenticatedEncryptionEncryptRequest()
        {
            SymmetricKey = symmetricKey,
            PayLoad = userPersistence,
            SchemaVersion = EncryptionSchemaVersions.V1,
            Nonce = nonce,

        };
        var encryptionResult = _encryptionService.Encrypt(encryptionRequest);

        var storageModel = UserEncryptedPersistenceModel.Create(user.TenantId, user.Id, encryptionResult.ComposedEncryptedPayload);
        return storageModel;
    }

    private async Task<User> DecryptEncryptedUser(UserEncryptedPersistenceModel userEncrypted, CancellationToken cancellationToken)
    {
        var symmetricKey = await _tenancySymmetricKeyService.GetExistingTenantSymmetricEncryptionKeyAsync(userEncrypted.TenantId, cancellationToken);
        var dectyptRequest = new AuthenticatedEncryptionDecryptRequest()
        {
            SchemaVersion = EncryptionSchemaVersions.V1,
            SymmetricKey = symmetricKey,
            ComposedEncryptedPayload = userEncrypted.EncryptedPayload,
        };
        var userPersisted = _encryptionService.Decrypt<UserPersistenceDto>(dectyptRequest);
        return userPersisted.ToDomain();
    }
}