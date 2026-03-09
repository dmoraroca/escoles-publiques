using Domain.Entities;

namespace Domain.Interfaces;
/// <summary>
/// Centralizes persistent data access for i student.
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
