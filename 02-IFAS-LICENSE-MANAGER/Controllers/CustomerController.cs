using IFAS.LicenseManager.DTOs;
using IFAS.LicenseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace IFAS.LicenseManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly LicenseService _licenseService;

    public CustomerController(LicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAll()
    {
        var customers = await _licenseService.GetCustomersAsync();

        var response = customers.Select(x => new CustomerResponse
        {
            Id = x.Id,
            CustomerId = x.CustomerId,
            CompanyName = x.CompanyName,
            ContactPerson = x.ContactPerson,
            Email = x.Email,
            Phone = x.Phone,
            Country = x.Country,
            CreatedAtUtc = x.CreatedAtUtc,
            IsActive = x.IsActive
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> Get(int id)
    {
        var customer = await _licenseService.GetCustomerAsync(id);

        if (customer == null)
            return NotFound(new { message = "Customer not found." });

        return Ok(new CustomerResponse
        {
            Id = customer.Id,
            CustomerId = customer.CustomerId,
            CompanyName = customer.CompanyName,
            ContactPerson = customer.ContactPerson,
            Email = customer.Email,
            Phone = customer.Phone,
            Country = customer.Country,
            CreatedAtUtc = customer.CreatedAtUtc,
            IsActive = customer.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(
        [FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customer = await _licenseService.CreateCustomerAsync(
                request.CompanyName,
                request.ContactPerson,
                request.Email,
                request.Phone,
                request.Address,
                request.Country);

            var response = new CustomerResponse
            {
                Id = customer.Id,
                CustomerId = customer.CustomerId,
                CompanyName = customer.CompanyName,
                ContactPerson = customer.ContactPerson,
                Email = customer.Email,
                Phone = customer.Phone,
                Country = customer.Country,
                CreatedAtUtc = customer.CreatedAtUtc,
                IsActive = customer.IsActive
            };

            return CreatedAtAction(
                nameof(Get),
                new { id = customer.Id },
                response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CreateCustomerRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
