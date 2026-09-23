namespace IFAS.VMS.Security.Tokens;

public sealed class JwtTokenOptions
{
    public string Issuer { get; set; } = "IFAS-VMS";
    public string Audience { get; set; } = "IFAS-VMS-Client";
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
