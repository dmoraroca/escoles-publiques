using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Web.Services.Api;

namespace Web.Controllers;

/// <summary>
/// Controlador d'autenticació d'usuaris: login i gestió de sessions.
/// </summary>
public class AuthController : Controller
{
    private readonly IAuthApiClient _authApiClient;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Constructor del controlador d'autenticació.
    /// </summary>
    public AuthController(IAuthApiClient authApiClient, ILogger<AuthController> logger)
    {
        _authApiClient = authApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Mostra el formulari de login.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        // Si ja està autenticat, redirigir segons el rol
        if (User.Identity?.IsAuthenticated == true)
        {
            var role = User.FindFirstValue("Role");
            return role == "ADM" ? RedirectToAction("Index", "Home") : RedirectToAction("Dashboard", "Dashboard");
        }

        return View();
    }

    /// <summary>
    /// Processa l'autenticació de l'usuari.
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Email i contrasenya són obligatoris";
                return View();
            }

            var token = await _authApiClient.GetTokenAsync(email, password);
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Credencials incorrectes";
                return View();
            }

            HttpContext.Session.SetString(SessionKeys.ApiToken, token);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
            var fullName = email;

            // Crear claims per l'usuari
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, fullName),
                new Claim("Role", role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

            _logger.LogInformation("User {Email} logged in with role {Role}", email, role);

            // Redirigir segons el rol
            if (role == "ADM")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }
        catch (Npgsql.NpgsqlException)
        {
            // Error de connexió a la base de dades
            return View("~/Views/Shared/ErrorDb.cshtml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperat al login");
            return View("~/Views/Shared/ErrorDb.cshtml");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        HttpContext.Session.Remove(SessionKeys.ApiToken);
        return RedirectToAction("Login");
    }
}
