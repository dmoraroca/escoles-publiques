using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;

    public AuthController(IAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] LoginDto dto)
    {
        var (success, userId, role) = await _authService.AuthenticateAsync(dto.Email, dto.Password);
        if (!success) return Unauthorized();
  
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key") ?? "dev-secret-key-please-change";
        var issuer = jwtSection.GetValue<string>("Issuer") ?? "EscolesApi";
        var audience = jwtSection.GetValue<string>("Audience") ?? "EscolesClients";
 
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, userId ?? string.Empty),
            new Claim(ClaimTypes.Role, role ?? string.Empty)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}

public record LoginDto(string Email, string Password);
