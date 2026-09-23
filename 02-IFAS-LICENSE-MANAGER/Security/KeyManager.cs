using IFAS.LicenseManager.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace IFAS.LicenseManager.Security;

public class KeyManager
{
    private readonly SecuritySettings _settings;
    private readonly string _keysDirectory;

    public KeyManager(IOptions<SecuritySettings> settings, IHostEnvironment environment)
    {
        _settings = settings.Value;
        _keysDirectory = Path.Combine(environment.ContentRootPath, "Keys");
        Directory.CreateDirectory(_keysDirectory);
    }

    public string PrivateKeyPath =>
        Path.Combine(_keysDirectory, _settings.PrivateKeyFileName);

    public string PublicKeyPath =>
        Path.Combine(_keysDirectory, _settings.PublicKeyFileName);

    public void EnsureKeyPairExists()
    {
        if (File.Exists(PrivateKeyPath) && File.Exists(PublicKeyPath))
            return;

        using var rsa = RSA.Create(_settings.KeySize);

        var privateKey = rsa.ExportPkcs8PrivateKeyPem();
        var publicKey = rsa.ExportSubjectPublicKeyInfoPem();

        File.WriteAllText(PrivateKeyPath, privateKey);
        File.WriteAllText(PublicKeyPath, publicKey);
    }

    public string GetPrivateKey()
    {
        EnsureKeyPairExists();
        return File.ReadAllText(PrivateKeyPath);
    }

    public string GetPublicKey()
    {
        EnsureKeyPairExists();
        return File.ReadAllText(PublicKeyPath);
    }
}
