namespace IFAS.LicenseManager.DTOs;

public class CustomerResponse
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public bool IsActive { get; set; }
}
