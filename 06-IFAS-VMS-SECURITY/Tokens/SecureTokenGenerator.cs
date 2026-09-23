using System.Security.Cryptography;

namespace IFAS.VMS.Security.Tokens;

public static class SecureTokenGenerator
{
    public static string GenerateBase64(int byteLength = 32)
    {
        if (byteLength < 16)
            throw new ArgumentOutOfRangeException(nameof(byteLength));

        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(byteLength));
    }

    public static string GenerateHex(int byteLength = 32)
    {
        if (byteLength < 16)
            throw new ArgumentOutOfRangeException(nameof(byteLength));

        return Convert.ToHexString(RandomNumberGenerator.GetBytes(byteLength));
    }
}
