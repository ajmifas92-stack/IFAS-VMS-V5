using IFAS.Server.Data; using IFAS.Server.DTOs; using IFAS.Server.Models; using Microsoft.EntityFrameworkCore;
namespace IFAS.Server.Services;
public sealed class CameraService(IFASDbContext db)
{
 public Task<List<CameraDto>> ListAsync()=>db.Cameras.Select(x=>new CameraDto(x.Id,x.Name,x.IpAddress,x.Protocol,x.StreamUrl,x.Enabled)).ToListAsync();
 public async Task<CameraDto> CreateAsync(CreateCameraRequest req){var c=new Camera{Name=req.Name,IpAddress=req.IpAddress,Protocol=req.Protocol,StreamUrl=req.StreamUrl,Username=req.Username,Password=req.Password}; db.Cameras.Add(c); await db.SaveChangesAsync(); return new CameraDto(c.Id,c.Name,c.IpAddress,c.Protocol,c.StreamUrl,c.Enabled);}
}
