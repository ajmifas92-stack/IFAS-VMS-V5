using System.Security.Cryptography;
using System.Text;

namespace IFAS.LicenseManager.Security;

public class SignatureService
{
    private readonly KeyManager _keyManager;

    public SignatureService(KeyManager keyManager)
    {
        _keyManager = keyManager;
    }

    public string Sign(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Data cannot be empty.", nameof(data));

        var privateKey = _keyManager.GetPrivateKey();

        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        var dataBytes = Encoding.UTF8.GetBytes(data);

        var signature = rsa.SignData(
            dataBytes,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signature);
    }

    public bool Verify(string data, string signature)
    {
        if (string.IsNullOrWhiteSpace(data) ||
            string.IsNullOrWhiteSpace(signature))
            return false;

        try
        {
            var publicKey = _keyManager.GetPublicKey();

            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);

            return rsa.VerifyData(
                dataBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }

    public string GetPublicKey()
    {
        return _keyManager.GetPublicKey();
    }
}
