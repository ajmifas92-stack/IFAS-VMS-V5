namespace IFAS.VMS.Client.Models;

public sealed class AppConfig
{
    public string DefaultRecordingRoot { get; set; } = "";
    public string DefaultSnapshotRoot { get; set; } = "";
    public List<CameraProfile> Cameras { get; set; } = new();
}
