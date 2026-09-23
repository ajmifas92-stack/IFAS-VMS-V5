using IFAS.Server.Models;
using Microsoft.EntityFrameworkCore;
namespace IFAS.Server.Data;
public sealed class IFASDbContext(DbContextOptions<IFASDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>(); public DbSet<Role> Roles => Set<Role>(); public DbSet<Permission> Permissions => Set<Permission>(); public DbSet<RolePermission> RolePermissions => Set<RolePermission>(); public DbSet<Camera> Cameras => Set<Camera>(); public DbSet<License> Licenses => Set<License>(); public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(x=>x.Username).IsUnique(); b.Entity<License>().HasIndex(x=>x.LicenseId).IsUnique(); b.Entity<Permission>().HasIndex(x=>x.Code).IsUnique();
        b.Entity<RolePermission>().HasKey(x=>new {x.RoleId,x.PermissionId});
        b.Entity<RolePermission>().HasOne(x=>x.Role).WithMany(x=>x.RolePermissions).HasForeignKey(x=>x.RoleId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<RolePermission>().HasOne(x=>x.Permission).WithMany(x=>x.RolePermissions).HasForeignKey(x=>x.PermissionId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<User>().HasOne(x=>x.Role).WithMany(x=>x.Users).HasForeignKey(x=>x.RoleId).OnDelete(DeleteBehavior.Restrict);
    }
}
