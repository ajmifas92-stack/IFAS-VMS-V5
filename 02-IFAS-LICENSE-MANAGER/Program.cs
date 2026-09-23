using IFAS.LicenseManager.Data;
using IFAS.LicenseManager.Services;
using IFAS.LicenseManager.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IFAS.LicenseManager.Configuration.LicenseSettings>(
    builder.Configuration.GetSection("LicenseSettings"));

builder.Services.Configure<IFAS.LicenseManager.Configuration.SecuritySettings>(
    builder.Configuration.GetSection("SecuritySettings"));

builder.Services.AddDbContext<LicenseDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("LicenseDatabase")
        ?? "Data Source=Data/ifas-license-manager.db";

    options.UseSqlite(connectionString);
});

builder.Services.AddScoped<LicenseGenerator>();
builder.Services.AddScoped<LicenseSigner>();
builder.Services.AddScoped<LicenseService>();
builder.Services.AddScoped<LicenseExporter>();

builder.Services.AddSingleton<KeyManager>();
builder.Services.AddSingleton<SignatureService>();
builder.Services.AddSingleton<LicenseCrypto>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

var dataDirectory = Path.Combine(app.Environment.ContentRootPath, "Data");
var outputDirectory = Path.Combine(app.Environment.ContentRootPath, "Output", "Licenses");
var keysDirectory = Path.Combine(app.Environment.ContentRootPath, "Keys");

Directory.CreateDirectory(dataDirectory);
Directory.CreateDirectory(outputDirectory);
Directory.CreateDirectory(keysDirectory);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LicenseDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("==============================================");
Console.WriteLine("        IFAS LICENSE MANAGER");
Console.WriteLine("==============================================");
Console.WriteLine("Application started successfully.");
Console.WriteLine("License Manager is running.");
Console.WriteLine("==============================================");

app.Run();
