namespace IFAS.VMS.Client.Models;

public sealed class AppConfig
{
    public string DefaultRecordingRoot { get; set; } = "";
    public string DefaultSnapshotRoot { get; set; } = "";

    // Recording retention
    public int RecordingRetentionDays { get; set; } = 30;
    public bool UseCustomRetentionDays { get; set; } = false;
    public int CustomRetentionDays { get; set; } = 30;

    // Recording expiry warnings
    public bool RetentionWarning7Days { get; set; } = true;
    public bool RetentionWarning3Days { get; set; } = true;
    public bool RetentionWarning1Day { get; set; } = false;
    public bool RetentionWarningCustom { get; set; } = false;
    public int CustomRetentionWarningDays { get; set; } = 14;

    public List<CameraProfile> Cameras { get; set; } = new();
}
