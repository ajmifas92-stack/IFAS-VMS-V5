namespace IFAS.LicenseManager.Models;

public class LicenseFeature
{
    public int Id { get; set; }
    public string FeatureCode { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}
