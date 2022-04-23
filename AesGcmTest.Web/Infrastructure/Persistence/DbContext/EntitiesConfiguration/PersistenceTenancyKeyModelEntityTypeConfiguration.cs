using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AesGcmTest.Infrastructure;

internal class PersistenceTenancyKeyModelEntityTypeConfiguration : IEntityTypeConfiguration<PersistenceTenancyKeyModel>
{
    public void Configure(EntityTypeBuilder<PersistenceTenancyKeyModel> builder)
    {
        builder.ToTable(EntityFrameworkConfigurationConstants.TenantsSymmetricKeysTable, EntityFrameworkConfigurationConstants.MainSchema);
        builder.HasKey(m => m.Id);

        builder.Property(m => m.TenantId);
        builder.Property(m => m.HsmKeyId);
        builder.Property(m => m.AesGcmEncryptedKey);
        builder.Property(m => m.IsActive);

        builder.HasIndex(m => m.TenantId).IsUnique(false);
        builder.HasIndex(m => m.HsmKeyId).IsUnique(true);
    }
}
