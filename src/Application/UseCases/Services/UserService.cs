using Application.Interfaces;
using Domain.DomainExceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;
/// <summary>
/// Implements application logic for user operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<UserService> _logger;
            /// <summary>
            /// Initializes a new instance of the UserService class with its required dependencies.
            /// </summary>
            public UserService(IUserRepository userRepository, IAuthService authService, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _authService = authService;
        _logger = logger;
    }
            /// <summary>
            /// Retrieves user by id async and returns it to the caller.
            /// </summary>
            public async Task<Domain.Entities.User?> GetUserByIdAsync(long id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
            /// <summary>
            /// Retrieves user by email async and returns it to the caller.
            /// </summary>
            public async Task<Domain.Entities.User?> GetUserByEmailAsync(string email)
    {
        var emailAddress = EmailAddress.Create(email);
        return await _userRepository.GetByEmailAsync(emailAddress.Value);
    }
            /// <summary>
            /// Creates user async by applying the required business rules.
            /// </summary>
            public async Task<Domain.Entities.User> CreateUserAsync(Domain.Entities.User user, string password)
    {
        var emailAddress = EmailAddress.Create(user.Email);
        user.Email = emailAddress.Value;

        // Validar que no existeixi un usuari amb aquest email
        var existingUser = await _userRepository.GetByEmailAsync(emailAddress.Value);
        if (existingUser != null)
        {
            throw new DuplicateEntityException("User", "Email", emailAddress.Value);
        }

        // Hashear password
        user.PasswordHash = _authService.HashPassword(password);
        user.Role = "USER"; // Per defecte els alumnes són USER
        user.IsActive = true;
        user.CreatedAt = DateTime.UtcNow;

        _logger.LogInformation("Creant nou usuari: {Email}", emailAddress.Value);
        return await _userRepository.AddAsync(user);
    }
            /// <summary>
            /// Updates user async with the data received in the request.
            /// </summary>
            public async Task UpdateUserAsync(Domain.Entities.User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.Id);
        if (existingUser == null)
        {
            throw new NotFoundException("User", user.Id);
        }

        user.Email = EmailAddress.Create(user.Email).Value;

        _logger.LogInformation("Actualitzant usuari: {UserId}", user.Id);
        await _userRepository.UpdateAsync(user);
    }
}
