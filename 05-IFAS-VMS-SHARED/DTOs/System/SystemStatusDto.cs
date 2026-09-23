namespace IFAS.VMS.Shared.DTOs.System;

public sealed class SystemStatusDto
{
    public string Product { get; set; } = "IFAS VMS";
    public string Version { get; set; } = "1.0.0";
    public string ServerStatus { get; set; } = "Unknown";
    public string LicenseStatus { get; set; } = "Unknown";
    public int UserCount { get; set; }
    public int CameraCount { get; set; }
    public DateTime ServerTimeUtc { get; set; }
}
