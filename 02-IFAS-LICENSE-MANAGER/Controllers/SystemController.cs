using IFAS.LicenseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace IFAS.LicenseManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly LicenseSigner _licenseSigner;

    public SystemController(LicenseSigner licenseSigner)
    {
        _licenseSigner = licenseSigner;
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            application = "IFAS License Manager",
            product = "IFAS VMS",
            status = "Running",
            utcTime = DateTime.UtcNow,
            version = "1.0.0"
        });
    }

    [HttpGet("public-key")]
    public IActionResult PublicKey()
    {
        return Content(
            _licenseSigner.GetPublicKey(),
            "text/plain");
    }
}
