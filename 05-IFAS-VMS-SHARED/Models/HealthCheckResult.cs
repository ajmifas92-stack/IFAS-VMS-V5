namespace IFAS.VMS.Shared.Models;

public sealed class HealthCheckResult
{
    public bool Healthy { get; set; }
    public string Component { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CheckedAtUtc { get; set; } = DateTime.UtcNow;
}
