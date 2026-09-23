using System.Security.Cryptography;
using IFAS.Server.Data; using IFAS.Server.Models; using IFAS.Server.Security; using Microsoft.EntityFrameworkCore;
namespace IFAS.Server.Services;
public sealed class SeedService(IFASDbContext db,PasswordHasher hasher)
{
 public async Task InitializeAsync(){
  await db.Database.EnsureCreatedAsync();
  var roleNames=new[]{"SuperAdmin","Administrator","Operator","Viewer"};
  foreach(var n in roleNames) if(!await db.Roles.AnyAsync(x=>x.Name==n)) db.Roles.Add(new Role{Name=n});
  var perms=new[]{"system.read","users.read","users.write","cameras.read","cameras.write","licenses.read","licenses.write","audit.read"};
  foreach(var c in perms) if(!await db.Permissions.AnyAsync(x=>x.Code==c)) db.Permissions.Add(new Permission{Code=c});
  await db.SaveChangesAsync();
  var adminRole=await db.Roles.SingleAsync(x=>x.Name=="SuperAdmin");
  if(!await db.Users.AnyAsync()){
   var password=Environment.GetEnvironmentVariable("IFAS_ADMIN_PASSWORD");
   if(string.IsNullOrWhiteSpace(password)){
    password=Convert.ToBase64String(RandomNumberGenerator.GetBytes(18)).Replace("/","_").Replace("+","-").TrimEnd('=');
    var bootstrap=Path.Combine(AppContext.BaseDirectory,"bootstrap-admin.txt");
    await File.WriteAllTextAsync(bootstrap,$"IFAS VMS bootstrap administrator\nUsername: admin\nPassword: {password}\nChange this password after first login.\n");
   }
   var h=hasher.Hash(password); db.Users.Add(new User{Username="admin",PasswordHash=h.Hash,PasswordSalt=h.Salt,RoleId=adminRole.Id}); await db.SaveChangesAsync();
  }
 }
}
