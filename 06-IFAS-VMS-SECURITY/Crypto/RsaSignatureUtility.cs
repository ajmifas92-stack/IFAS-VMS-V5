using System.Security.Cryptography;
using System.Text;

namespace IFAS.VMS.Security.Crypto;

public static class RsaSignatureUtility
{
    public static string Sign(string content, RSA privateKey)
    {
        ArgumentNullException.ThrowIfNull(privateKey);

        var data = Encoding.UTF8.GetBytes(content);
        var signature = privateKey.SignData(
            data,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signature);
    }

    public static bool Verify(
        string content,
        string signatureBase64,
        RSA publicKey)
    {
        ArgumentNullException.ThrowIfNull(publicKey);

        try
        {
            var data = Encoding.UTF8.GetBytes(content);
            var signature = Convert.FromBase64String(signatureBase64);

            return publicKey.VerifyData(
                data,
                signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
