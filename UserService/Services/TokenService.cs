using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Models;
using UserService.Models.Configs;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class TokenService : ITokenService
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<Users> _userMgr;
    
    public TokenService(IOptions<JwtConfig> jwtSettings, UserManager<Users> userMgr)
    {
        _jwtConfig = jwtSettings.Value;
        _userMgr = userMgr;
    }
    
    public async Task<string> GenerateJwtToken(Users user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var userRole = _userMgr.GetRolesAsync(user).Result.First();

        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, userRole)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.ValidIssuer,
            audience: _jwtConfig.ValidAudience,
            expires: DateTime.Now.AddHours(2), // 2 hours expirty time 
            claims: claims,
            signingCredentials: credentials
        );

        return await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token));
    }
}