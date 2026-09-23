namespace IFAS.Server.Models;
public sealed class AuditLog { public long Id { get; set; } public string? Username { get; set; } public string Action { get; set; } = string.Empty; public string? Details { get; set; } public DateTime CreatedUtc { get; set; } = DateTime.UtcNow; public string? IpAddress { get; set; } }
