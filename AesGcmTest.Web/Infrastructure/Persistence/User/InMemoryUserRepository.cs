using AesGcmTest.Domain;
using System.Text;
using System.Text.Json;

namespace AesGcmTest.Infrastructure;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<UserEncryptedPersistenceModel> _usersStorage;
    private readonly ITenancySimmetricKeyService _tenancySimmetricKeyService;

    public InMemoryUserRepository(List<UserEncryptedPersistenceModel> usersStorage, ITenancySimmetricKeyService tenancySimmetricKeyService)
    {
        _usersStorage = usersStorage;
        _tenancySimmetricKeyService = tenancySimmetricKeyService;
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
        UserEncryptedPersistenceModel encryptedUser = await EncryptUserAsync(user, encryptedExistingUser.Nonce, cancellationToken);

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
        var encryptPayload = JsonSerializer.Serialize(userPersistence);

        var symmetricKey = await _tenancySimmetricKeyService.GetOrCreateTenantSymmetricEncryptionKeyAsync(user.TenantId, cancellationToken);

        var encryptPayloadBytes = Encoding.UTF8.GetBytes(encryptPayload);
        var encryptResult = AesGcmSymmetricEncryption.Encrypt(encryptPayloadBytes, symmetricKey, nonce);

        var storageModel = UserEncryptedPersistenceModel.Create(user.TenantId, user.Id, encryptResult.CipherTextInBytes, encryptResult.TagInBytes, nonce);
        return storageModel;
    }

    private async Task<User> DecryptEncryptedUser(UserEncryptedPersistenceModel userEncrypted, CancellationToken cancellationToken)
    {
        var symmetricKey = await _tenancySimmetricKeyService.GetOrCreateTenantSymmetricEncryptionKeyAsync(userEncrypted.TenantId, cancellationToken);
        var decryptPayload = AesGcmSymmetricEncryption.Decrypt(userEncrypted.EncryptedPayload, symmetricKey, userEncrypted.Nonce, userEncrypted.Tag);
        var str = Encoding.UTF8.GetString(decryptPayload.PlainTextInBytes);
        var userPersisted = JsonSerializer.Deserialize<UserPersistenceDto>(str) ?? throw new Exception("Fail to Deserialize");
        return userPersisted.ToDomain();
    }
}