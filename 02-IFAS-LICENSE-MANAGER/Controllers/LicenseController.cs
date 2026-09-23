using IFAS.LicenseManager.DTOs;
using IFAS.LicenseManager.Models;
using IFAS.LicenseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace IFAS.LicenseManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LicenseController : ControllerBase
{
    private readonly LicenseService _licenseService;

    public LicenseController(LicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LicenseResponse>>> GetAll()
    {
        var licenses = await _licenseService.GetLicensesAsync();

        var response = licenses.Select(x => new LicenseResponse
        {
            LicenseId = x.LicenseId,
            LicenseKey = x.LicenseKey,
            CustomerId = x.Customer?.CustomerId ?? string.Empty,
            CustomerName = x.Customer?.CompanyName ?? string.Empty,
            MaxUsers = x.MaxUsers,
            MaxCameras = x.MaxCameras,
            IssuedAtUtc = x.IssuedAtUtc,
            ExpiresAtUtc = x.ExpiresAtUtc,
            IsActive = x.IsActive,
            IsRevoked = x.IsRevoked,
            ServerBinding = x.ServerBinding,
            Features = System.Text.Json.JsonSerializer.Deserialize<List<string>>(x.FeaturesJson) ?? new(),
            LicenseFilePath = x.LicenseFilePath
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{licenseId}")]
    public async Task<ActionResult<LicenseResponse>> Get(string licenseId)
    {
        var license = await _licenseService.GetLicenseAsync(licenseId);

        if (license == null)
            return NotFound(new { message = "License not found." });

        var features =
            System.Text.Json.JsonSerializer.Deserialize<List<string>>(license.FeaturesJson)
            ?? new List<string>();

        return Ok(new LicenseResponse
        {
            LicenseId = license.LicenseId,
            LicenseKey = license.LicenseKey,
            CustomerId = license.Customer?.CustomerId ?? string.Empty,
            CustomerName = license.Customer?.CompanyName ?? string.Empty,
            MaxUsers = license.MaxUsers,
            MaxCameras = license.MaxCameras,
            IssuedAtUtc = license.IssuedAtUtc,
            ExpiresAtUtc = license.ExpiresAtUtc,
            IsActive = license.IsActive,
            IsRevoked = license.IsRevoked,
            ServerBinding = license.ServerBinding,
            Features = features,
            LicenseFilePath = license.LicenseFilePath
        });
    }

    [HttpPost]
    public async Task<ActionResult<LicenseResponse>> Create(
        [FromBody] CreateLicenseRequest request)
    {
        try
        {
            if (request.CustomerId <= 0)
                return BadRequest(new { message = "CustomerId is required." });

            if (request.MaxUsers <= 0)
                return BadRequest(new { message = "MaxUsers must be greater than zero." });

            if (request.MaxCameras <= 0)
                return BadRequest(new { message = "MaxCameras must be greater than zero." });

            if (request.ValidityDays <= 0)
                return BadRequest(new { message = "ValidityDays must be greater than zero." });

            var modelRequest = new LicenseRequest
            {
                MaxUsers = request.MaxUsers,
                MaxCameras = request.MaxCameras,
                ValidityDays = request.ValidityDays,
                ServerBinding = request.ServerBinding ?? string.Empty,
                Features = request.Features ?? new List<string>()
            };

            var license = await _licenseService.CreateLicenseAsync(
                request.CustomerId,
                modelRequest);

            var features =
                System.Text.Json.JsonSerializer.Deserialize<List<string>>(license.FeaturesJson)
                ?? new List<string>();

            var response = new LicenseResponse
            {
                LicenseId = license.LicenseId,
                LicenseKey = license.LicenseKey,
                CustomerId = license.Customer?.CustomerId ?? string.Empty,
                CustomerName = license.Customer?.CompanyName ?? string.Empty,
                MaxUsers = license.MaxUsers,
                MaxCameras = license.MaxCameras,
                IssuedAtUtc = license.IssuedAtUtc,
                ExpiresAtUtc = license.ExpiresAtUtc,
                IsActive = license.IsActive,
                IsRevoked = license.IsRevoked,
                ServerBinding = license.ServerBinding,
                Features = features,
                LicenseFilePath = license.LicenseFilePath
            };

            return CreatedAtAction(
                nameof(Get),
                new { licenseId = license.LicenseId },
                response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{licenseId}/revoke")]
    public async Task<IActionResult> Revoke(
        string licenseId,
        [FromBody] RevokeLicenseRequest request)
    {
        var success = await _licenseService.RevokeLicenseAsync(
            licenseId,
            request.Reason);

        if (!success)
            return NotFound(new { message = "License not found." });

        return Ok(new
        {
            message = "License revoked successfully.",
            licenseId
        });
    }
}

public class RevokeLicenseRequest
{
    public string Reason { get; set; } = string.Empty;
}
