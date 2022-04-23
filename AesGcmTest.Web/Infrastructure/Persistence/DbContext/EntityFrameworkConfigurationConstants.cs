namespace AesGcmTest.Infrastructure;

public static class EntityFrameworkConfigurationConstants
{
    public const string MainSchema = "Core";
    public const string HsmSchema = "LocalHsm";

    public const string UsersTable = "Users";
    public const string TenantsSymmetricKeysTable = "TenantsSymmetricKeys";
    public const string RsaKeys = "RsaKeys";
}
