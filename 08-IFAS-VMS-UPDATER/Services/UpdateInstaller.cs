namespace IFAS.VMS.Updater.Services;

public sealed class UpdateInstaller
{
    public void InstallStagedDirectory(
        string stagedDirectory,
        string installDirectory,
        string backupDirectory)
    {
        if (!Directory.Exists(stagedDirectory))
            throw new DirectoryNotFoundException(stagedDirectory);

        Directory.CreateDirectory(
            Path.GetDirectoryName(installDirectory)
            ?? throw new InvalidOperationException("Invalid install directory."));

        if (Directory.Exists(installDirectory))
            Directory.Move(installDirectory, backupDirectory);

        try
        {
            Directory.Move(stagedDirectory, installDirectory);
        }
        catch
        {
            if (!Directory.Exists(installDirectory) &&
                Directory.Exists(backupDirectory))
                Directory.Move(backupDirectory, installDirectory);

            throw;
        }
    }
}
