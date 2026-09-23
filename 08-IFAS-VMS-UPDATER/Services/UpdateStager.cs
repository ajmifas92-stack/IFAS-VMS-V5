namespace IFAS.VMS.Updater.Services;

public sealed class UpdateStager
{
    public Task<string> CreateStagingDirectoryAsync(
        string stagingRoot,
        string version,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(stagingRoot, $"{version}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return Task.FromResult(path);
    }

    public void ExtractPackage(string packageFile, string stagingDirectory)
    {
        if (!File.Exists(packageFile))
            throw new FileNotFoundException("Update package not found.", packageFile);

        Directory.CreateDirectory(stagingDirectory);

        if (Directory.EnumerateFileSystemEntries(stagingDirectory).Any())
            throw new InvalidOperationException("Staging directory must be empty.");

        System.IO.Compression.ZipFile.ExtractToDirectory(
            packageFile, stagingDirectory, overwriteFiles: true);
    }
}
