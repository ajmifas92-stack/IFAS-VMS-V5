using System.Security.Cryptography;
using System.Text;

namespace IFAS.LicenseManager.Security;

public class LicenseCrypto
{
    public string GenerateLicenseKey()
    {
        var randomBytes = new byte[24];
        RandomNumberGenerator.Fill(randomBytes);

        var value = Convert.ToHexString(randomBytes);
        return $"IFAS-{FormatKey(value)}";
    }

    public string GenerateLicenseId()
    {
        return $"IFAS-LIC-{Guid.NewGuid():N}".ToUpperInvariant();
    }

    public string GenerateCustomerId()
    {
        return $"IFAS-CUS-{Guid.NewGuid():N}".ToUpperInvariant();
    }

    public string ComputeSha256(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string FormatKey(string value)
    {
        var parts = new List<string>();

        for (var i = 0; i < value.Length; i += 8)
        {
            var length = Math.Min(8, value.Length - i);
            parts.Add(value.Substring(i, length));
        }

        return string.Join("-", parts);
    }
}
