namespace IFAS.LicenseManager.DTOs;

public class LicenseResponse
{
    public string LicenseId { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxCameras { get; set; }
    public DateTime IssuedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsActive { get; set; }
    public bool IsRevoked { get; set; }
    public string ServerBinding { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public string LicenseFilePath { get; set; } = string.Empty;
}
