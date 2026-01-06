using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interfície de repositori per gestionar escoles al domini.
/// </summary>
public interface ISchoolRepository
{
    Task<IEnumerable<School>> GetAllAsync();
    Task<School?> GetByIdAsync(long id);
    Task<School?> GetByCodeAsync(string code);
    Task<School> AddAsync(School school);
    Task UpdateAsync(School school);
    Task DeleteAsync(long id);
}
