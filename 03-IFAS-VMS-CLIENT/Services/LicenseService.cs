namespace IFAS.VMS.Client.Services;

public sealed record LicenseStatusDto(
    bool Valid,
    string? Customer,
    int MaxUsers,
    int MaxCameras,
    DateTime? ExpiresUtc,
    string? Message);

public sealed record InstallLicenseRequest(
    string SignedLicenseJson);

public sealed class LicenseService
{
    private readonly ApiClient _api;

    public LicenseService(ApiClient api)
    {
        _api = api;
    }

    public Task<LicenseStatusDto?> GetStatusAsync(
        CancellationToken cancellationToken = default)
    {
        return _api.GetAsync<LicenseStatusDto>(
            "api/license/status",
            cancellationToken);
    }

    public Task<LicenseStatusDto?> InstallAsync(
        string signedLicenseJson,
        CancellationToken cancellationToken = default)
    {
        var request = new InstallLicenseRequest(signedLicenseJson);

        return _api.PostAsync<InstallLicenseRequest, LicenseStatusDto>(
            "api/license/install",
            request,
            cancellationToken);
    }
}
