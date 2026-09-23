namespace IFAS.LicenseManager.Configuration;

public class SecuritySettings
{
    public string SignatureAlgorithm { get; set; } = "RSA-SHA256";
    public int KeySize { get; set; } = 3072;
    public string PrivateKeyFileName { get; set; } = "ifas-license-private.pem";
    public string PublicKeyFileName { get; set; } = "ifas-license-public.pem";
    public bool RequireDigitalSignature { get; set; } = true;
}
