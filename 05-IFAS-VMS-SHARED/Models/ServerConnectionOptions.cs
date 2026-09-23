namespace IFAS.VMS.Shared.Models;

public sealed class ServerConnectionOptions
{
    public string BaseUrl { get; set; } = "https://localhost:8443";
    public int TimeoutSeconds { get; set; } = 30;
    public bool AllowInvalidDevelopmentCertificate { get; set; } = true;
}
