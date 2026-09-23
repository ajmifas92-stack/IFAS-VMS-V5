namespace IFAS.LicenseManager.Models;

public class LicenseRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int MaxUsers { get; set; } = 10;
    public int MaxCameras { get; set; } = 32;
    public int ValidityDays { get; set; } = 365;
    public string ServerBinding { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}
