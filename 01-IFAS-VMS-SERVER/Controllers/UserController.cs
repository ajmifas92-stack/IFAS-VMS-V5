using IFAS.Server.DTOs; using IFAS.Server.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc;
namespace IFAS.Server.Controllers;
[ApiController][Route("api/users")][Authorize]
public sealed class UserController(UserService service):ControllerBase
{ [HttpGet] public Task<List<UserDto>> List()=>service.ListAsync(); [HttpPost][Authorize(Roles="SuperAdmin,Administrator")] public async Task<IActionResult> Create(CreateUserRequest req)=>Ok(await service.CreateAsync(req)); }
