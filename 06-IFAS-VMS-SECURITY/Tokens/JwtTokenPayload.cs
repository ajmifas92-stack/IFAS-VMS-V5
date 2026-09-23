namespace IFAS.VMS.Security.Tokens;

public sealed class JwtTokenPayload
{
    public string Subject { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string[] Permissions { get; set; } = [];
}
