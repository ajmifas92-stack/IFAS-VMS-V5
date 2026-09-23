namespace IFAS.VMS.Shared.DTOs.Cameras;

public sealed class CameraDto
{
    public string CameraId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 554;
    public string Protocol { get; set; } = "RTSP";
    public string StreamUrl { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string Status { get; set; } = "Unknown";
}
