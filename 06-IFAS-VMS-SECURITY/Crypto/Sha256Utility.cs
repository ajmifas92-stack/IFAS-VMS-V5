using System.Security.Cryptography;
using System.Text;

namespace IFAS.VMS.Security.Crypto;

public static class Sha256Utility
{
    public static string ComputeHex(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string ComputeBase64(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return Convert.ToBase64String(SHA256.HashData(data));
    }
}
