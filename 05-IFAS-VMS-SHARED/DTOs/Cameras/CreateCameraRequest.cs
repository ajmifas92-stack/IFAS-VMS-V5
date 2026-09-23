namespace IFAS.VMS.Shared.DTOs.Cameras;

public sealed class CreateCameraRequest
{
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 554;
    public string Protocol { get; set; } = "RTSP";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string StreamUrl { get; set; } = string.Empty;
}
