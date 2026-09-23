using System.Security.Cryptography;

namespace IFAS.VMS.Security.Keys;

public static class RsaKeyGenerator
{
    public static (string PrivateKeyPem, string PublicKeyPem) Generate(int keySize = 3072)
    {
        using var rsa = RSA.Create(keySize);

        return (
            rsa.ExportRSAPrivateKeyPem(),
            rsa.ExportRSAPublicKeyPem()
        );
    }
}
