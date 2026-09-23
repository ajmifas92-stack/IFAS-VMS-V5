using IFAS.Server.Data; using IFAS.Server.DTOs; using IFAS.Server.Security; using Microsoft.EntityFrameworkCore;
namespace IFAS.Server.Services;
public sealed class AuthService(IFASDbContext db,PasswordHasher hasher,JwtService jwt)
{
 public async Task<LoginResponse?> LoginAsync(LoginRequest req)
 { var u=await db.Users.Include(x=>x.Role).SingleOrDefaultAsync(x=>x.Username==req.Username && x.IsActive); if(u is null || !hasher.Verify(req.Password,u.PasswordHash,u.PasswordSalt)) return null; return new LoginResponse(jwt.Create(u.Id,u.Username,u.Role.Name),u.Id,u.Username,u.Role.Name); }
}
