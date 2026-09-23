namespace IFAS.Server.DTOs;
public record LoginRequest(string Username,string Password);
public record LoginResponse(string AccessToken,int UserId,string Username,string Role);
