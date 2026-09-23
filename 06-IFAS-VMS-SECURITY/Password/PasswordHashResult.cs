namespace IFAS.VMS.Security.Password;

public sealed class PasswordHashResult
{
    public string Hash { get; init; } = string.Empty;
    public string Salt { get; init; } = string.Empty;
    public int Iterations { get; init; }
}
