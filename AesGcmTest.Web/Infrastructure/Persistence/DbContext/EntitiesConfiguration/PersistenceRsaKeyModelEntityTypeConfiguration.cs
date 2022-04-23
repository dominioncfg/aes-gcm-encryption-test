using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AesGcmTest.Infrastructure;

internal class PersistenceRsaKeyModelEntityTypeConfiguration : IEntityTypeConfiguration<PersistenceRsaKeyModel>
{
    public void Configure(EntityTypeBuilder<PersistenceRsaKeyModel> builder)
    {
        builder.ToTable(EntityFrameworkConfigurationConstants.RsaKeys, EntityFrameworkConfigurationConstants.HsmSchema);
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FriendlyKeyId);
        builder.Property(m => m.PrivateKey);
        builder.Property(m => m.PublicKey);

        builder.HasIndex(m => m.FriendlyKeyId).IsUnique(true);
    }
}