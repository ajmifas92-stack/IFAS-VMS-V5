namespace IFAS.Server.DTOs;
public record CreateCameraRequest(string Name,string IpAddress,string Protocol,string StreamUrl,string Username,string Password);
public record CameraDto(int Id,string Name,string IpAddress,string Protocol,string StreamUrl,bool Enabled);
