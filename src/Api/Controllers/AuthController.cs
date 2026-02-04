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
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _config;

    public AuthController(ILogger<AuthController> logger, IAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;   
        _logger = logger;
    
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] LoginDto dto)
    {
        try
        {
            _logger.LogInformation("Auth token request received. Email={Email}", dto.Email);
            
            // API
            var (success, userId, role) = await _authService.AuthenticateAsync(dto.Email, dto.Password);
            _logger.LogInformation("Auth result. Success={Success}, UserId={UserId}, Role={Role}", success, userId ?? "null", role ?? "null");
            if (!success) return Unauthorized();
    
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key") ?? "dev-secret-key-please-change";
            var issuer = jwtSection.GetValue<string>("Issuer") ?? "EscolesApi";
            var audience = jwtSection.GetValue<string>("Audience") ?? "EscolesClients";
            _logger.LogDebug("JWT settings. Issuer={Issuer}, Audience={Audience}, KeyLength={KeyLength}",
                issuer, audience, key.Length);
    
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

            _logger.LogInformation("JWT generated. Expires={ExpiresUtc}", token.ValidTo);
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generant token d'autenticació. Email={Email}", dto?.Email ?? "null");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error intern del servidor");
            throw;
        }
    }
}

public record LoginDto(string Email, string Password);
