using Domain.Entities;

namespace Web.Services.Api;
/// <summary>
/// Defines the contract required by i schools api client components.
/// </summary>
public interface ISchoolsApiClient
{
    Task<IEnumerable<School>> GetAllAsync();
    Task<School?> GetByIdAsync(long id);
    Task<School> CreateAsync(School school);
    Task UpdateAsync(long id, School school);
    Task DeleteAsync(long id);
}
