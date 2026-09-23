using IFAS.Server.Data; using IFAS.Server.DTOs; using IFAS.Server.Models; using IFAS.Server.Security; using Microsoft.EntityFrameworkCore;
namespace IFAS.Server.Services;
public sealed class UserService(IFASDbContext db,PasswordHasher hasher)
{
 public async Task<List<UserDto>> ListAsync()=>await db.Users.Include(x=>x.Role).Select(x=>new UserDto(x.Id,x.Username,x.Role.Name,x.IsActive)).ToListAsync();
 public async Task<UserDto> CreateAsync(CreateUserRequest req){var role=await db.Roles.SingleOrDefaultAsync(x=>x.Name==req.Role) ?? await db.Roles.SingleAsync(); if(await db.Users.AnyAsync(x=>x.Username==req.Username)) throw new InvalidOperationException("Username already exists."); var p=hasher.Hash(req.Password); var u=new User{Username=req.Username,RoleId=role.Id,PasswordHash=p.Hash,PasswordSalt=p.Salt}; db.Users.Add(u); await db.SaveChangesAsync(); return new UserDto(u.Id,u.Username,role.Name,u.IsActive);}
}
