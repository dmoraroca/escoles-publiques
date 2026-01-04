using Domain.Entities;

namespace Application.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentByIdAsync(long id);
    Task<IEnumerable<Student>> GetStudentsBySchoolIdAsync(long schoolId);
    Task<Student> CreateStudentAsync(Student student);
    Task UpdateStudentAsync(Student student);
    Task DeleteStudentAsync(long id);
}
