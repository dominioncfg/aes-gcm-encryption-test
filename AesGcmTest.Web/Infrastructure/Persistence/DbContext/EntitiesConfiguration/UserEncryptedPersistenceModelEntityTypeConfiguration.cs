using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AesGcmTest.Infrastructure;

internal class UserEncryptedPersistenceModelEntityTypeConfiguration : IEntityTypeConfiguration<UserEncryptedPersistenceModel>
{
    public void Configure(EntityTypeBuilder<UserEncryptedPersistenceModel> builder)
    {
        builder.ToTable(EntityFrameworkConfigurationConstants.UsersTable, EntityFrameworkConfigurationConstants.MainSchema);
        builder.HasKey(m => m.Id);

        builder.Property(m => m.TenantId);
        builder.Property(m => m.EncryptedPayload);
    }
}
