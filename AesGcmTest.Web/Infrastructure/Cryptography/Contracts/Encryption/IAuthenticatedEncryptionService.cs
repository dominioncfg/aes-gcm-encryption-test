namespace AesGcmTest.Infrastructure;

public interface IAuthenticatedEncryptionService
{
    AuthenticatedEncryptionEncryptResponse Encrypt(AuthenticatedEncryptionEncryptRequest request);

    T Decrypt<T>(AuthenticatedEncryptionDecryptRequest request);
}
