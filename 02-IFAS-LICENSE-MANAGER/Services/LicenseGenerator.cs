using System.Text.Json;
using IFAS.LicenseManager.Models;
using IFAS.LicenseManager.Security;

namespace IFAS.LicenseManager.Services;

public class LicenseGenerator
{
    private readonly LicenseCrypto _crypto;
    private readonly LicenseSigner _signer;

    public LicenseGenerator(LicenseCrypto crypto, LicenseSigner signer)
    {
        _crypto = crypto;
        _signer = signer;
    }

    public License Generate(Customer customer, LicenseRequest request)
    {
        if (request.MaxUsers <= 0)
            throw new ArgumentException("MaxUsers must be greater than zero.");

        if (request.MaxCameras <= 0)
            throw new ArgumentException("MaxCameras must be greater than zero.");

        if (request.ValidityDays <= 0)
            throw new ArgumentException("ValidityDays must be greater than zero.");

        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddDays(request.ValidityDays);

        var licenseId = _crypto.GenerateLicenseId();
        var licenseKey = _crypto.GenerateLicenseKey();

        var payloadObject = new
        {
            LicenseId = licenseId,
            LicenseKey = licenseKey,
            CustomerId = customer.CustomerId,
            CustomerName = customer.CompanyName,
            ContactPerson = customer.ContactPerson,
            MaxUsers = request.MaxUsers,
            MaxCameras = request.MaxCameras,
            IssuedAtUtc = issuedAt,
            ExpiresAtUtc = expiresAt,
            ServerBinding = request.ServerBinding,
            Features = request.Features
        };

        var payload = JsonSerializer.Serialize(payloadObject);

        var signature = _signer.SignPayload(payload);

        return new License
        {
            LicenseId = licenseId,
            LicenseKey = licenseKey,
            CustomerId = customer.Id,
            Customer = customer,
            MaxUsers = request.MaxUsers,
            MaxCameras = request.MaxCameras,
            IssuedAtUtc = issuedAt,
            ExpiresAtUtc = expiresAt,
            ServerBinding = request.ServerBinding,
            FeaturesJson = JsonSerializer.Serialize(request.Features),
            LicensePayload = payload,
            Signature = signature,
            IsActive = true,
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
