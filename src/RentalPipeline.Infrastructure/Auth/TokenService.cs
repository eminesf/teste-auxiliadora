using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentalPipeline.Application.Interfaces;
using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Infrastructure.Auth;

public class TokenService(IConfiguration configuration) : ITokenService
{
  public string GenerateToken(Client client)
  {
    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));

    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
            new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new Claim(ClaimTypes.Email, client.Email),
            new Claim(ClaimTypes.Name, client.Name),
            new Claim(ClaimTypes.Role, client.Role),
        };

    var token = new JwtSecurityToken(
        issuer: configuration["Jwt:Issuer"],
        audience: configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(8),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}