namespace IFAS.LicenseManager.Models;

public class License
{
    public int Id { get; set; }
    public string LicenseId { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int MaxUsers { get; set; }
    public int MaxCameras { get; set; }
    public DateTime IssuedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsRevoked { get; set; } = false;
    public string ServerBinding { get; set; } = string.Empty;
    public string FeaturesJson { get; set; } = "[]";
    public string LicensePayload { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public string LicenseFilePath { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }
    public string RevocationReason { get; set; } = string.Empty;
}
