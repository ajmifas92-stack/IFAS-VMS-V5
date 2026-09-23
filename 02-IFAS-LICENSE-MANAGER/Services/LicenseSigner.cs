using IFAS.LicenseManager.Security;

namespace IFAS.LicenseManager.Services;

public class LicenseSigner
{
    private readonly SignatureService _signatureService;

    public LicenseSigner(SignatureService signatureService)
    {
        _signatureService = signatureService;
    }

    public string SignPayload(string payload)
    {
        return _signatureService.Sign(payload);
    }

    public bool VerifyPayload(string payload, string signature)
    {
        return _signatureService.Verify(payload, signature);
    }

    public string GetPublicKey()
    {
        return _signatureService.GetPublicKey();
    }
}
