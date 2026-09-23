using IFAS.Server.DTOs; using IFAS.Server.Services; using Microsoft.AspNetCore.Mvc;
namespace IFAS.Server.Controllers;
[ApiController][Route("api/auth")]
public sealed class AuthController(AuthService auth):ControllerBase
{ [HttpPost("login")] public async Task<IActionResult> Login(LoginRequest req){var r=await auth.LoginAsync(req); return r is null?Unauthorized(new{message="Invalid username or password."}):Ok(r);} }
