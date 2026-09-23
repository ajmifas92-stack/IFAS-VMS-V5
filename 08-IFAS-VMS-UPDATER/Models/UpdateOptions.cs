namespace IFAS.VMS.Updater.Models;

public sealed class UpdateOptions
{
    public string InstallRoot { get; set; } = @"C:\Program Files\IFAS\IFAS VMS";
    public string DataRoot { get; set; } = @"C:\ProgramData\IFAS\IFAS VMS";
    public string StagingRoot { get; set; } = @"C:\ProgramData\IFAS\IFAS VMS\Updates\Staging";
    public string BackupRoot { get; set; } = @"C:\ProgramData\IFAS\IFAS VMS\Updates\Backups";
    public int DownloadTimeoutSeconds { get; set; } = 120;
}
