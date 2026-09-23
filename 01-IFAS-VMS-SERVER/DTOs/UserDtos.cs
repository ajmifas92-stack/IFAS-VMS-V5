namespace IFAS.Server.DTOs;
public record CreateUserRequest(string Username,string Password,string Role="Viewer");
public record UserDto(int Id,string Username,string Role,bool IsActive);
