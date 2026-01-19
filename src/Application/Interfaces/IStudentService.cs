using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Interfície de servei per gestionar alumnes a l'aplicació.
/// </summary>
public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentByIdAsync(long id);
    Task<IEnumerable<Student>> GetStudentsBySchoolIdAsync(long schoolId);
    Task<Student> CreateStudentAsync(Student student);
    Task<Student> CreateStudentWithUserAsync(Domain.Entities.User user, string password, Student student);
    Task UpdateStudentAsync(Student student);
    Task UpdateStudentWithUserAsync(Student student, Domain.Entities.User user);
    Task DeleteStudentAsync(long id);
}
