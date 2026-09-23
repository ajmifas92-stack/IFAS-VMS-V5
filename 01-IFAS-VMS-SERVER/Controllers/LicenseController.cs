using IFAS.Server.DTOs; using IFAS.Server.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc;
namespace IFAS.Server.Controllers;
[ApiController][Route("api/license")]
public sealed class LicenseController(LicenseService service):ControllerBase
{ [HttpGet("status")] public Task<LicenseStatusDto> Status()=>service.GetStatusAsync(); [HttpPost("install")][Authorize(Roles="SuperAdmin,Administrator")] public async Task<IActionResult> Install(InstallLicenseRequest req)=>Ok(await service.InstallAsync(req)); }
