namespace IFAS.VMS.Updater.Services;

public sealed class RollbackService
{
    public void Rollback(string installDirectory, string backupDirectory)
    {
        if (Directory.Exists(installDirectory))
            Directory.Delete(installDirectory, true);

        if (Directory.Exists(backupDirectory))
            Directory.Move(backupDirectory, installDirectory);
    }
}
