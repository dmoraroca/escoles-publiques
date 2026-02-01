using Domain.Entities;

namespace Web.Services.Api;

public interface ISchoolsApiClient
{
    Task<IEnumerable<School>> GetAllAsync();
    Task<School?> GetByIdAsync(long id);
    Task<School> CreateAsync(School school);
    Task UpdateAsync(long id, School school);
    Task DeleteAsync(long id);
}
