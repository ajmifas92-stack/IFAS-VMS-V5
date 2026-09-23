using IFAS.Server.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using IFAS.Server.Data;
namespace IFAS.Server.Controllers;
[ApiController][Route("api/system")]
public sealed class SystemController(IFASDbContext db):ControllerBase
{ [HttpGet("health")] public IActionResult Health()=>Ok(new {status="ok",utc=DateTime.UtcNow,version="0.1.0"}); [HttpGet("stats")][Authorize] public async Task<IActionResult> Stats()=>Ok(new{users=await db.Users.CountAsync(x=>x.IsActive),cameras=await db.Cameras.CountAsync(x=>x.Enabled),licensed=await db.Licenses.AnyAsync(x=>!x.Revoked)}); }
