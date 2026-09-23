namespace IFAS.Server.DTOs;
public record InstallLicenseRequest(string SignedLicenseJson);
public record LicenseStatusDto(bool Valid,string? Customer,int MaxUsers,int MaxCameras,DateTime? ExpiresUtc,string? Message);
