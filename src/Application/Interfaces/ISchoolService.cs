using Domain.Entities;

namespace Application.Interfaces;
/// <summary>
/// Implements application logic for i school operations.
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
