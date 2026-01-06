namespace Application.Interfaces;

/// <summary>
/// Interfície de servei per gestionar autenticació d'usuaris.
/// </summary>
public interface IAuthService
{
    Task<(bool success, string? token, string? role)> AuthenticateAsync(string email, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
