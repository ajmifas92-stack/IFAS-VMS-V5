namespace IFAS.VMS.Client.Models;

public sealed class CameraProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Camera";
    public string Host { get; set; } = "";
    public int Port { get; set; } = 554;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string RtspUrl { get; set; } = "";
    public string RecordingPath { get; set; } = "";
    public bool RecordEnabled { get; set; }
    public string Protocol { get; set; } = "RTSP";
}
