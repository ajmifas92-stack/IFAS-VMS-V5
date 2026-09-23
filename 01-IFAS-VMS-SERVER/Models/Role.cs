namespace IFAS.Server.Models;
public sealed class Role { public int Id { get; set; } public string Name { get; set; } = "Viewer"; public ICollection<User> Users { get; set; } = new List<User>(); public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>(); }
