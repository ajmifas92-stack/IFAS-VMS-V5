using System.Text.Json;
using IFAS.LicenseManager.Models;

namespace IFAS.LicenseManager.Services;

public class LicenseExporter
{
    private readonly IWebHostEnvironment _environment;

    public LicenseExporter(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string Export(License license)
    {
        var outputDirectory = Path.Combine(
            _environment.ContentRootPath,
            "Output",
            "Licenses");

        Directory.CreateDirectory(outputDirectory);

        var fileName = $"{license.LicenseId}.ifaslic";
        var filePath = Path.Combine(outputDirectory, fileName);

        var licenseFile = new
        {
            FormatVersion = 1,
            LicenseId = license.LicenseId,
            LicenseKey = license.LicenseKey,
            Payload = license.LicensePayload,
            Signature = license.Signature
        };

        var json = JsonSerializer.Serialize(
            licenseFile,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(filePath, json);
        return filePath;
    }
}
