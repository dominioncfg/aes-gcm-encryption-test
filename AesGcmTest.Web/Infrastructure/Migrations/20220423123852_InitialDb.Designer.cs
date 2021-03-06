// <auto-generated />
using System;
using AesGcmTest.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AesGcmTest.Web.Infrastructure.Migrations
{
    [DbContext(typeof(AesGcmDbContext))]
    [Migration("20220423123852_InitialDb")]
    partial class InitialDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AesGcmTest.Infrastructure.PersistenceRsaKeyModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FriendlyKeyId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("PrivateKey")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PublicKey")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("FriendlyKeyId")
                        .IsUnique();

                    b.ToTable("RsaKeys", "LocalHsm");
                });

            modelBuilder.Entity("AesGcmTest.Infrastructure.PersistenceTenancyKeyModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("AesGcmEncryptedKey")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("HsmKeyId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("HsmKeyId")
                        .IsUnique();

                    b.HasIndex("TenantId");

                    b.ToTable("TenantsSymmetricKeys", "Core");
                });

            modelBuilder.Entity("AesGcmTest.Infrastructure.UserEncryptedPersistenceModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("EncryptedPayload")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Users", "Core");
                });
#pragma warning restore 612, 618
        }
    }
}
