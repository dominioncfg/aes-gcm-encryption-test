using AesGcmTest.Domain;

namespace AesGcmTest.Infrastructure;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<UserEncryptedPersistenceModel> _usersStorage;
    private readonly ITenancySimmetricKeyService _tenancySimmetricKeyService;
    private readonly IAuthenticatedEncryptionService _encryptionService;

    public InMemoryUserRepository(List<UserEncryptedPersistenceModel> usersStorage, 
        ITenancySimmetricKeyService tenancySimmetricKeyService,
        IAuthenticatedEncryptionService encryptionService)
    {
        _usersStorage = usersStorage;
        _tenancySimmetricKeyService = tenancySimmetricKeyService;
        _encryptionService = encryptionService;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        var nonce = AesGcmSymmetricEncryption.GetRandomNonce();
        UserEncryptedPersistenceModel encryptedUser = await EncryptUserAsync(user, nonce.Bytes, cancellationToken);
        _usersStorage.Add(encryptedUser);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var persistenceUserIndex = _usersStorage.FindIndex(x => x.Id == user.Id);
        if (persistenceUserIndex == -1)
            throw new Exception("User not found");

        var encryptedExistingUser = _usersStorage[persistenceUserIndex];
        var nonce = AesGcmSymmetricEncryption.GetRandomNonce();
        UserEncryptedPersistenceModel encryptedUser = await EncryptUserAsync(user, nonce.Bytes, cancellationToken);

        _usersStorage[persistenceUserIndex] = encryptedUser;
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var userEncrypted = _usersStorage.FirstOrDefault(x => x.Id == id) ?? throw new Exception("User not found");
        return await DecryptEncryptedUser(userEncrypted, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenantEncryptedUsers = _usersStorage
            .Where(x => x.TenantId == tenantId)
            .ToList();

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
        var symmetricKey = await _tenancySimmetricKeyService.GetOrCreateTenantSymmetricEncryptionKeyAsync(user.TenantId, cancellationToken);
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
        var symmetricKey = await _tenancySimmetricKeyService.GetExistingTenantSymmetricEncryptionKeyAsync(userEncrypted.TenantId, cancellationToken);
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