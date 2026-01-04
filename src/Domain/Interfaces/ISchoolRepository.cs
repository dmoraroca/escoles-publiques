using Domain.Entities;

namespace Domain.Interfaces;

public interface ISchoolRepository
{
    Task<IEnumerable<School>> GetAllAsync();
    Task<School?> GetByIdAsync(long id);
    Task<School?> GetByCodeAsync(string code);
    Task<School> AddAsync(School school);
    Task UpdateAsync(School school);
    Task DeleteAsync(long id);
}
