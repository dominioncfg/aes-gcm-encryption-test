using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AesGcmTest.Web.Infrastructure.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LocalHsm");

            migrationBuilder.EnsureSchema(
                name: "Core");

            migrationBuilder.CreateTable(
                name: "RsaKeys",
                schema: "LocalHsm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendlyKeyId = table.Column<string>(type: "text", nullable: false),
                    PrivateKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    PublicKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsaKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantsSymmetricKeys",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    HsmKeyId = table.Column<string>(type: "text", nullable: false),
                    AesGcmEncryptedKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantsSymmetricKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EncryptedPayload = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RsaKeys_FriendlyKeyId",
                schema: "LocalHsm",
                table: "RsaKeys",
                column: "FriendlyKeyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantsSymmetricKeys_HsmKeyId",
                schema: "Core",
                table: "TenantsSymmetricKeys",
                column: "HsmKeyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantsSymmetricKeys_TenantId",
                schema: "Core",
                table: "TenantsSymmetricKeys",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RsaKeys",
                schema: "LocalHsm");

            migrationBuilder.DropTable(
                name: "TenantsSymmetricKeys",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Core");
        }
    }
}
