namespace IFAS.VMS.Security.Policies;

public static class SecurityPolicy
{
    public const int MinimumPasswordLength = 8;
    public const int RecommendedPasswordLength = 12;

    public const int MinimumJwtSecretBytes = 32;

    public const int MaxLoginAttempts = 5;
    public const int LoginLockoutMinutes = 15;

    public const string PasswordAlgorithm = "PBKDF2-SHA256";
    public const string LicenseSignatureAlgorithm = "RSA-SHA256";
}
