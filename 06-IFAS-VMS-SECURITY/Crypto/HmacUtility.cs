using System.Security.Cryptography;
using System.Text;

namespace IFAS.VMS.Security.Crypto;

public static class HmacUtility
{
    public static string ComputeSha256Base64(string secret, string value)
    {
        ArgumentNullException.ThrowIfNull(secret);
        ArgumentNullException.ThrowIfNull(value);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    public static bool VerifySha256Base64(
        string secret,
        string value,
        string expectedBase64)
    {
        var actual = ComputeSha256Base64(secret, value);

        try
        {
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(actual),
                Convert.FromBase64String(expectedBase64));
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
