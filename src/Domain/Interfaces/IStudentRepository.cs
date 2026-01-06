using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interfície de repositori per gestionar alumnes al domini.
/// </summary>
public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(long id);
    Task<IEnumerable<Student>> GetBySchoolIdAsync(long schoolId);
    Task<Student> AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(long id);
}
