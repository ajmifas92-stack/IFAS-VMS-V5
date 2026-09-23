namespace IFAS.Server.Configuration;
public sealed class SecuritySettings
{
    public string JwtIssuer { get; set; } = "IFAS-VMS";
    public string JwtAudience { get; set; } = "IFAS-VMS-Client";
    public string JwtSecret { get; set; } = "CHANGE-ME";
    public int TokenMinutes { get; set; } = 60;
    public string PublicLicenseKeyPath { get; set; } = "Keys/license-public.pem";
}
