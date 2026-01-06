namespace Application.Interfaces;

/// <summary>
/// Interfície de servei per gestionar usuaris a l'aplicació.
/// </summary>
public interface IUserService
{
    Task<Domain.Entities.User?> GetUserByIdAsync(long id);
    Task<Domain.Entities.User?> GetUserByEmailAsync(string email);
    Task<Domain.Entities.User> CreateUserAsync(Domain.Entities.User user, string password);
    Task UpdateUserAsync(Domain.Entities.User user);
}
