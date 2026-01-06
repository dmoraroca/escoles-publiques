using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        // Si ja està autenticat, redirigir segons el rol
        if (User.Identity?.IsAuthenticated == true)
        {
            var role = User.FindFirstValue("Role");
            return role == "ADM" ? RedirectToAction("Index", "Home") : RedirectToAction("Dashboard", "User");
        }

        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            TempData["Error"] = "Email i contrasenya són obligatoris";
            return View();
        }

        var (success, userId, role) = await _authService.AuthenticateAsync(email, password);

        if (!success)
        {
            TempData["Error"] = "Credencials incorrectes";
            return View();
        }

        // Obtenir l'usuari per agafar el nom complet
        var userRepository = HttpContext.RequestServices.GetRequiredService<Domain.Interfaces.IUserRepository>();
        var user = await userRepository.GetByIdAsync(long.Parse(userId!));
        var fullName = user != null ? $"{user.FirstName} {user.LastName}" : email;

        // Crear claims per l'usuari
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId!),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, fullName),
            new Claim("Role", role!)
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
            return RedirectToAction("Dashboard", "User");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }
}
