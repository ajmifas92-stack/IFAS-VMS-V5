namespace IFAS.Server.Models;
public sealed class Permission { public int Id { get; set; } public string Code { get; set; } = string.Empty; public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>(); }
