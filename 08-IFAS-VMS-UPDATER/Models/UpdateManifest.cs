namespace IFAS.VMS.Updater.Models;

public sealed class UpdateManifest
{
    public string Product { get; set; } = "IFAS VMS";
    public string Version { get; set; } = string.Empty;
    public string ReleaseDateUtc { get; set; } = string.Empty;
    public string PackageUrl { get; set; } = string.Empty;
    public string PackageSha256 { get; set; } = string.Empty;
    public string PackageSignature { get; set; } = string.Empty;
    public string MinimumSupportedVersion { get; set; } = string.Empty;
    public bool Mandatory { get; set; }
    public string[] ReleaseNotes { get; set; } = [];
}
