using IFAS.LicenseManager.Data;
using IFAS.LicenseManager.Models;
using Microsoft.EntityFrameworkCore;

namespace IFAS.LicenseManager.Services;

public class LicenseService
{
    private readonly LicenseDbContext _db;
    private readonly LicenseGenerator _generator;
    private readonly LicenseExporter _exporter;

    public LicenseService(
        LicenseDbContext db,
        LicenseGenerator generator,
        LicenseExporter exporter)
    {
        _db = db;
        _generator = generator;
        _exporter = exporter;
    }

    public async Task<Customer> CreateCustomerAsync(
        string companyName,
        string contactPerson,
        string email,
        string phone,
        string address,
        string country)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name is required.");

        var customer = new Customer
        {
            CustomerId = $"IFAS-CUS-{Guid.NewGuid():N}".ToUpperInvariant(),
            CompanyName = companyName.Trim(),
            ContactPerson = contactPerson?.Trim() ?? string.Empty,
            Email = email?.Trim() ?? string.Empty,
            Phone = phone?.Trim() ?? string.Empty,
            Address = address?.Trim() ?? string.Empty,
            Country = country?.Trim() ?? string.Empty,
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = true
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task<License> CreateLicenseAsync(
        int customerId,
        LicenseRequest request)
    {
        var customer = await _db.Customers
            .FirstOrDefaultAsync(x => x.Id == customerId);

        if (customer == null)
            throw new InvalidOperationException("Customer was not found.");

        if (!customer.IsActive)
            throw new InvalidOperationException("Customer is inactive.");

        var license = _generator.Generate(customer, request);

        var existing = await _db.Licenses
            .AnyAsync(x => x.LicenseId == license.LicenseId);

        if (existing)
            throw new InvalidOperationException("License ID already exists.");

        _db.Licenses.Add(license);
        await _db.SaveChangesAsync();

        var filePath = _exporter.Export(license);
        license.LicenseFilePath = filePath;

        await _db.SaveChangesAsync();
        return license;
    }

    public async Task<License?> GetLicenseAsync(string licenseId)
    {
        return await _db.Licenses
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.LicenseId == licenseId);
    }

    public async Task<List<License>> GetLicensesAsync()
    {
        return await _db.Licenses
            .Include(x => x.Customer)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<bool> RevokeLicenseAsync(
        string licenseId,
        string reason)
    {
        var license = await _db.Licenses
            .FirstOrDefaultAsync(x => x.LicenseId == licenseId);

        if (license == null)
            return false;

        license.IsActive = false;
        license.IsRevoked = true;
        license.RevokedAtUtc = DateTime.UtcNow;
        license.RevocationReason = reason?.Trim() ?? string.Empty;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _db.Customers
            .OrderBy(x => x.CompanyName)
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerAsync(int id)
    {
        return await _db.Customers
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
