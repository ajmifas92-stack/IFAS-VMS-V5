using IFAS.Server.DTOs; using IFAS.Server.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc;
namespace IFAS.Server.Controllers;
[ApiController][Route("api/cameras")][Authorize]
public sealed class CameraController(CameraService service):ControllerBase
{ [HttpGet] public Task<List<CameraDto>> List()=>service.ListAsync(); [HttpPost][Authorize(Roles="SuperAdmin,Administrator,Operator")] public async Task<IActionResult> Create(CreateCameraRequest req)=>Ok(await service.CreateAsync(req)); }
