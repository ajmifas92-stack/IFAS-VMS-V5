namespace IFAS.Server.DTOs;

public record LicenseStatusDto(
    bool Valid,
    string? Customer,
    int MaxUsers,
    int MaxCameras,
    int UsedCameras,
    int RemainingCameras,
    DateTime? ExpiresUtc,
    string? Message);

public record InstallLicenseRequest(
    string SignedLicenseJson);
