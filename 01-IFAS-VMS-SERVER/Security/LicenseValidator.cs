using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IFAS.Server.Models;
namespace IFAS.Server.Security;
public sealed class LicenseValidator
{
    private readonly string _publicKeyPath; public LicenseValidator(string publicKeyPath)=>_publicKeyPath=publicKeyPath;
    public bool IsWithinLimits(License license,int users,int cameras) => !license.Revoked && license.ExpiresUtc >= DateTime.UtcNow && users <= license.MaxUsers && cameras <= license.MaxCameras;
    public bool VerifySignature(string signedLicenseJson)
    { if (!File.Exists(_publicKeyPath)) return false; try { using var doc=JsonDocument.Parse(signedLicenseJson); var root=doc.RootElement; var payload=root.GetProperty("payload").GetRawText(); var signature=Convert.FromBase64String(root.GetProperty("signature").GetString()!); using var rsa=RSA.Create(); rsa.ImportFromPem(File.ReadAllText(_publicKeyPath)); return rsa.VerifyData(Encoding.UTF8.GetBytes(payload),signature,HashAlgorithmName.SHA256,RSASignaturePadding.Pss); } catch { return false; } }
}
