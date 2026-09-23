namespace IFAS.VMS.Shared.DTOs.License;

public sealed class LicenseSummary
{
    public string LicenseId { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxCameras { get; set; }
    public DateTime IssuedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsActive { get; set; }
    public bool IsRevoked { get; set; }
    public string Status { get; set; } = "Unknown";
    public string[] Features { get; set; } = [];
}
