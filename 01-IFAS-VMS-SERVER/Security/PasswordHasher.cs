using System.Security.Cryptography;
namespace IFAS.Server.Security;
public sealed class PasswordHasher
{
    public (string Hash,string Salt) Hash(string password)
    { var salt=RandomNumberGenerator.GetBytes(16); var hash=Rfc2898DeriveBytes.Pbkdf2(password,salt,120_000,HashAlgorithmName.SHA256,32); return (Convert.ToBase64String(hash),Convert.ToBase64String(salt)); }
    public bool Verify(string password,string hash,string salt)
    { try { var s=Convert.FromBase64String(salt); var expected=Convert.FromBase64String(hash); var actual=Rfc2898DeriveBytes.Pbkdf2(password,s,120_000,HashAlgorithmName.SHA256,32); return CryptographicOperations.FixedTimeEquals(actual,expected); } catch { return false; } }
}
