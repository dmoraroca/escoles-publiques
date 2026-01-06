using Application.Interfaces;
using Domain.DomainExceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

/// <summary>
/// Servei d'aplicació per gestionar usuaris: consulta, creació i actualització.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IAuthService authService, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Retorna un usuari pel seu identificador.
    /// </summary>
    public async Task<Domain.Entities.User?> GetUserByIdAsync(long id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Retorna un usuari pel seu email.
    /// </summary>
    public async Task<Domain.Entities.User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    /// <summary>
    /// Crea un nou usuari amb validació i hash de contrasenya.
    /// </summary>
    public async Task<Domain.Entities.User> CreateUserAsync(Domain.Entities.User user, string password)
    {
        // Validar que no existeixi un usuari amb aquest email
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        if (existingUser != null)
        {
            throw new DuplicateEntityException("User", "Email", user.Email);
        }

        // Hashear password
        user.PasswordHash = _authService.HashPassword(password);
        user.Role = "USER"; // Per defecte els alumnes són USER
        user.IsActive = true;
        user.CreatedAt = DateTime.UtcNow;

        _logger.LogInformation("Creant nou usuari: {Email}", user.Email);
        return await _userRepository.AddAsync(user);
    }

    /// <summary>
    /// Actualitza les dades d'un usuari existent.
    /// </summary>
    public async Task UpdateUserAsync(Domain.Entities.User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.Id);
        if (existingUser == null)
        {
            throw new NotFoundException("User", user.Id);
        }

        _logger.LogInformation("Actualitzant usuari: {UserId}", user.Id);
        await _userRepository.UpdateAsync(user);
    }
}
