# AES GCM Encryption Test App

A reference application for tenant-based encryption using:

- AES GCM symmetric encryption for encrypting content.
- Local key wrapping for storing the Aes keys encrypted with RSA. Support for Local, Azure Key Vault and Aws Kms Supported.
- Supports key rotation.
- All Keys and data are stored in PostgreSql.

## Using Local RSA Keys

    1. Go to AesGcmTest.Web/Hosting/Configuration/ConfigurationExtensions.cs
    2. Inside AddInfrastructure use this version: return services.AddCompletelyLocalHsmEncryptionStorage();

## Using Aws KMS Keys

    1. Go to AesGcmTest.Web/Hosting/Configuration/ConfigurationExtensions.cs
    2. Inside AddInfrastructure use this version: return services.AddWithAwsHsmTenancyEncryptionStorage(configuration);
    3. Update app config or User Secrets or use the provided Local Stack KMS in the docker compose.

## Using Azure Key Vault

    1. Go to AesGcmTest.Web/Hosting/Configuration/ConfigurationExtensions.cs
    2. Inside AddInfrastructure use this version: return services.AddWithAwsHsmTenancyEncryptionStorage(configuration);
    3. Update app config or User Secrets with the corresponding Key Vault setting. (You need to create a service provider inside azure and give permissions to your vault).

### Example Azure Key Vauyt Configuration

    "azureKeyVault": 
    {
        "VaultUrl": "https://<keyValutName>.vault.azure.net/",
        "TenantId": "....",
        "ClientId": "...",
        "ClientSecret": "..."
    }
