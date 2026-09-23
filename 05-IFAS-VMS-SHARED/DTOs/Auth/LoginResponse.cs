namespace IFAS.VMS.Shared.DTOs.Auth;

public sealed class LoginResponse
{
    public bool Success { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAtUtc { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string[] Permissions { get; set; } = [];
    public string Message { get; set; } = string.Empty;
}
