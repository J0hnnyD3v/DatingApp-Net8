using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key from appSettings.");
        if (tokenKey.Length < 64)
        {
            throw new Exception("Token key is too short, it needs to be longer.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        
        var claims = new List<Claim>
        {
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.NameIdentifier, $"{user.FirstName} {user.LastName}"),
        };
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
