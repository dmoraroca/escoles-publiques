using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interfície de repositori per gestionar usuaris al domini.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(long id);
    Task<bool> EmailExistsAsync(string email);
}
