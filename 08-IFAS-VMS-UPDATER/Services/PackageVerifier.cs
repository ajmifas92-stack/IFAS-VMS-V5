using System.Security.Cryptography;

namespace IFAS.VMS.Updater.Services;

public sealed class PackageVerifier
{
    public async Task<string> CalculateSha256Async(
        string filePath, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(filePath);
        using var sha = SHA256.Create();
        var hash = await sha.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public async Task<bool> VerifySha256Async(
        string filePath,
        string expectedSha256,
        CancellationToken cancellationToken = default)
    {
        var actual = await CalculateSha256Async(filePath, cancellationToken);

        try
        {
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(actual),
                Convert.FromHexString(expectedSha256.Trim()));
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
