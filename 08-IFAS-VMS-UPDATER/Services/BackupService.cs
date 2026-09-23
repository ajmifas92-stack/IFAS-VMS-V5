namespace IFAS.VMS.Updater.Services;

public sealed class BackupService
{
    public Task<string> CreateBackupAsync(
        string sourceDirectory,
        string backupRoot,
        string version,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException(sourceDirectory);

        var backupDirectory = Path.Combine(
            backupRoot, $"{version}-{DateTime.UtcNow:yyyyMMddHHmmss}");

        Directory.CreateDirectory(backupDirectory);
        CopyDirectory(sourceDirectory, backupDirectory);
        return Task.FromResult(backupDirectory);
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);

        foreach (var file in Directory.GetFiles(source))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);

        foreach (var directory in Directory.GetDirectories(source))
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
    }
}
