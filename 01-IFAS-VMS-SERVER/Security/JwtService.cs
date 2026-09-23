using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IFAS.Server.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace IFAS.Server.Security;
public sealed class JwtService(SecuritySettings settings)
{
    public string Create(int userId,string username,string role)
    { var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSecret)); var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256); var claims=new[]{new Claim(JwtRegisteredClaimNames.Sub,userId.ToString()),new Claim(ClaimTypes.Name,username),new Claim(ClaimTypes.Role,role)}; var token=new JwtSecurityToken(settings.JwtIssuer,settings.JwtAudience,claims,expires:DateTime.UtcNow.AddMinutes(settings.TokenMinutes),signingCredentials:creds); return new JwtSecurityTokenHandler().WriteToken(token); }
}
