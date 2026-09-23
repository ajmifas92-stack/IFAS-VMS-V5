using IFAS.Server.Data; using IFAS.Server.Models;
namespace IFAS.Server.Services;
public sealed class AuditService(IFASDbContext db){ public async Task WriteAsync(string action,string? user,string? details,string? ip){db.AuditLogs.Add(new AuditLog{Action=action,Username=user,Details=details,IpAddress=ip}); await db.SaveChangesAsync();}}
