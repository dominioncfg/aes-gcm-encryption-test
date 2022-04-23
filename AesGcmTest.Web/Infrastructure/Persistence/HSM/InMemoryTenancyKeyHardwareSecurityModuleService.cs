using Microsoft.EntityFrameworkCore;

namespace AesGcmTest.Infrastructure;

public class RsaKeyLocalRepository : IRsaKeyLocalRepository
{
    private readonly AesGcmDbContext _dbContext;

    public RsaKeyLocalRepository(AesGcmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PersistenceRsaKeyModel rsaKey, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(rsaKey, cancellationToken);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<PersistenceRsaKeyModel> GetByFriendlyIdAsync(string rsaKeyId, CancellationToken cancellationToken)
    {
        var key = await _dbContext.LocalHsmKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FriendlyKeyId == rsaKeyId, cancellationToken);

        if (key is null)
            throw new Exception("Rsa Key not found");

        return key;
    }
}
