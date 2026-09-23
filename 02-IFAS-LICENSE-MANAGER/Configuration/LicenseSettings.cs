namespace IFAS.LicenseManager.Configuration;

public class LicenseSettings
{
    public string ApplicationName { get; set; } = "IFAS VMS";
    public string LicenseManagerName { get; set; } = "IFAS License Manager";
    public int DefaultLicenseValidityDays { get; set; } = 365;
    public int DefaultMaxUsers { get; set; } = 10;
    public int DefaultMaxCameras { get; set; } = 32;
    public string LicenseFileExtension { get; set; } = ".ifaslic";
    public string OutputDirectory { get; set; } = "Output/Licenses";
    public string KeysDirectory { get; set; } = "Keys";
}
