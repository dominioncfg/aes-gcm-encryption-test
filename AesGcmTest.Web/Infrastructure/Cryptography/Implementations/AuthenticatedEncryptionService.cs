using System.Text;
using System.Text.Json;

namespace AesGcmTest.Infrastructure;

public class AuthenticatedEncryptionService : IAuthenticatedEncryptionService
{
    const int AesGcmNonceLengthInBytes = 12;
    const int AesGcmTagLengthInBytes = 16;

    public AuthenticatedEncryptionEncryptResponse Encrypt(AuthenticatedEncryptionEncryptRequest request)
    {
        switch (request.SchemaVersion)
        {
            case EncryptionSchemaVersions.V1:
                return EncryptVersion1(request);
            default:
                throw new InvalidOperationException("Not Supported version");
        }
    }

    public T Decrypt<T>(AuthenticatedEncryptionDecryptRequest request)
    {
        switch (request.SchemaVersion)
        {
            case EncryptionSchemaVersions.V1:
                return DecryptVersion1<T>(request);
            default:
                throw new InvalidOperationException("Not Supported version");
        }
    }

    private static AuthenticatedEncryptionEncryptResponse EncryptVersion1(AuthenticatedEncryptionEncryptRequest request)
    {
        var encryptPayload = JsonSerializer.Serialize(request.PayLoad);
        var encryptPayloadBytes = Encoding.UTF8.GetBytes(encryptPayload);
        var encryptResult = AesGcmSymmetricEncryption.Encrypt(encryptPayloadBytes, request.SymmetricKey, request.Nonce);

        var composedResultPayload = new byte[encryptResult.TagInBytes.Length + encryptResult.CipherTextInBytes.Length + request.Nonce.Length];
        encryptResult.TagInBytes.CopyTo(composedResultPayload, 0);
        encryptResult.CipherTextInBytes.CopyTo(composedResultPayload, encryptResult.TagInBytes.Length);
        request.Nonce.CopyTo(composedResultPayload, encryptResult.TagInBytes.Length + encryptResult.CipherTextInBytes.Length);

        return new AuthenticatedEncryptionEncryptResponse()
        {
            ComposedEncryptedPayload = composedResultPayload,
        };
    }

    private static T DecryptVersion1<T>(AuthenticatedEncryptionDecryptRequest request)
    {
        var tag = new byte[AesGcmTagLengthInBytes];
        var cipherText = new byte[request.ComposedEncryptedPayload.Length - AesGcmTagLengthInBytes - AesGcmNonceLengthInBytes];
        var nonce = new byte[AesGcmNonceLengthInBytes];

        Array.Copy(request.ComposedEncryptedPayload, tag, tag.Length);
        Array.Copy(request.ComposedEncryptedPayload, tag.Length, cipherText, 0, cipherText.Length);
        Array.Copy(request.ComposedEncryptedPayload, tag.Length + cipherText.Length, nonce, 0, nonce.Length);


        var decryptPayload = AesGcmSymmetricEncryption.Decrypt(cipherText, request.SymmetricKey, nonce, tag);
        var str = Encoding.UTF8.GetString(decryptPayload.PlainTextInBytes);
        var model = JsonSerializer.Deserialize<T>(str) ?? throw new Exception("Fail to Deserialize");
        return model;
    }

}
