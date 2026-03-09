namespace Application.Interfaces;
/// <summary>
/// Defines authentication operations for credential validation and password hashing.
/// </summary>
public interface IAuthService
{
    Task<(bool success, string? token, string? role)> AuthenticateAsync(string email, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
