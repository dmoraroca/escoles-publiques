using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Interfície de servei per gestionar escoles a l'aplicació.
/// </summary>
public interface ISchoolService
{
    Task<IEnumerable<School>> GetAllSchoolsAsync();
    Task<School?> GetSchoolByIdAsync(long id);
    Task<School?> GetSchoolByCodeAsync(string code);
    Task<School> CreateSchoolAsync(School school);
    Task UpdateSchoolAsync(School school);
    Task DeleteSchoolAsync(long id);
}
