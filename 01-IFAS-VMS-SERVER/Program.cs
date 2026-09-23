using System.Text;
using IFAS.Server.Configuration; using IFAS.Server.Data; using IFAS.Server.Middleware; using IFAS.Server.Security; using IFAS.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; using Microsoft.EntityFrameworkCore; using Microsoft.IdentityModel.Tokens;
var builder=WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService(options => options.ServiceName = "IFAS VMS Server");
var serverSettings=builder.Configuration.GetSection("Server").Get<ServerSettings>() ?? new();
var securitySettings=builder.Configuration.GetSection("Security").Get<SecuritySettings>() ?? new();
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(serverSettings.DatabasePath))!);
builder.Services.AddSingleton(serverSettings); builder.Services.AddSingleton(securitySettings);
builder.Services.AddDbContext<IFASDbContext>(o=>o.UseSqlite($"Data Source={serverSettings.DatabasePath}"));
builder.Services.AddSingleton<PasswordHasher>(); builder.Services.AddSingleton<JwtService>(); builder.Services.AddSingleton(new LicenseValidator(securitySettings.PublicLicenseKeyPath));
builder.Services.AddScoped<AuthService>(); builder.Services.AddScoped<UserService>(); builder.Services.AddScoped<CameraService>(); builder.Services.AddScoped<LicenseService>(); builder.Services.AddScoped<AuditService>(); builder.Services.AddScoped<SeedService>();
builder.Services.AddControllers(); builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o=>{o.TokenValidationParameters=new TokenValidationParameters{ValidateIssuer=true,ValidateAudience=true,ValidateIssuerSigningKey=true,ValidateLifetime=true,ValidIssuer=securitySettings.JwtIssuer,ValidAudience=securitySettings.JwtAudience,IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitySettings.JwtSecret)),ClockSkew=TimeSpan.FromMinutes(1)};});
builder.Services.AddAuthorization();
var app=builder.Build();
app.UseMiddleware<ExceptionMiddleware>(); app.UseMiddleware<SecurityMiddleware>(); app.UseSwagger(); app.UseSwaggerUI(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers();
using(var scope=app.Services.CreateScope()){await scope.ServiceProvider.GetRequiredService<SeedService>().InitializeAsync();}
app.Run($"http://{serverSettings.Host}:{serverSettings.Port}");
