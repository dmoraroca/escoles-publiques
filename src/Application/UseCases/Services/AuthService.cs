using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.UseCases.Services;
/// <summary>
/// Implements application logic for auth operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
        /// <summary>
        /// Initializes a new instance of the AuthService class with its required dependencies.
        /// </summary>
        public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

        public async Task<(bool success, string? token, string? role)> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || !user.IsActive)
        {
            return (false, null, null);
        }

        var passwordHash = HashPassword(password);

        if (user.PasswordHash != passwordHash)
        {
            return (false, null, null);
        }

        // Actualitzar last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Retornar èxit amb el rol de l'usuari
        return (true, user.Id.ToString(), user.Role);
    }
        /// <summary>
        /// Executes the hash password operation as part of this component.
        /// </summary>
        public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
        /// <summary>
        /// Executes the verify password operation as part of this component.
        /// </summary>
        public bool VerifyPassword(string password, string passwordHash)
    {
        var hash = HashPassword(password);
        return hash == passwordHash;
    }
}
