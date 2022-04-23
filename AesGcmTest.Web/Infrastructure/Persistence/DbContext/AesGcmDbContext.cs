using Microsoft.EntityFrameworkCore;

namespace AesGcmTest.Infrastructure;

public class AesGcmDbContext : DbContext
{
    public DbSet<UserEncryptedPersistenceModel> Users => Set<UserEncryptedPersistenceModel>();
    public DbSet<PersistenceTenancyKeyModel> TenantsSymmetricKeys => Set<PersistenceTenancyKeyModel>();
    public DbSet<PersistenceRsaKeyModel> LocalHsmKeys => Set<PersistenceRsaKeyModel>();

    public AesGcmDbContext(DbContextOptions<AesGcmDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AddEntitiesConfiguration(modelBuilder);
    }

    private static void AddEntitiesConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEncryptedPersistenceModelEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PersistenceTenancyKeyModelEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PersistenceRsaKeyModelEntityTypeConfiguration());
    }

}
