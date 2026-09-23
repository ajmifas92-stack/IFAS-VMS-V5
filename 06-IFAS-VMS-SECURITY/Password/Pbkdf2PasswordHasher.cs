using System.Security.Cryptography;

namespace IFAS.VMS.Security.Password;

public sealed class Pbkdf2PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int DefaultIterations = 210_000;

    public PasswordHashResult Hash(string password, int iterations = DefaultIterations)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return new PasswordHashResult
        {
            Hash = Convert.ToBase64String(hash),
            Salt = Convert.ToBase64String(salt),
            Iterations = iterations
        };
    }

    public bool Verify(
        string password,
        string storedHash,
        string storedSalt,
        int iterations = DefaultIterations)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(storedHash) ||
            string.IsNullOrWhiteSpace(storedSalt))
            return false;

        try
        {
            var salt = Convert.FromBase64String(storedSalt);
            var expected = Convert.FromBase64String(storedHash);

            var actual = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
