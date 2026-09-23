namespace IFAS.VMS.Shared.Models;

public sealed class LicenseLimits
{
    public int MaxUsers { get; set; }
    public int MaxCameras { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
}
