namespace IFAS.LicenseManager.DTOs;

public class CreateLicenseRequest
{
    public int CustomerId { get; set; }
    public int MaxUsers { get; set; } = 10;
    public int MaxCameras { get; set; } = 32;
    public int ValidityDays { get; set; } = 365;
    public string ServerBinding { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}
