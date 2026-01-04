using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<StudentService> _logger;

    public StudentService(IStudentRepository studentRepository, ILogger<StudentService> logger)
    {
        _studentRepository = studentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        try
        {
            return await _studentRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtenir tots els alumnes");
            throw;
        }
    }

    public async Task<Student?> GetStudentByIdAsync(long id)
    {
        return await _studentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Student>> GetStudentsBySchoolIdAsync(long schoolId)
    {
        return await _studentRepository.GetBySchoolIdAsync(schoolId);
    }

    public async Task<Student> CreateStudentAsync(Student student)
    {
        student.CreatedAt = DateTime.UtcNow;
        return await _studentRepository.AddAsync(student);
    }

    public async Task UpdateStudentAsync(Student student)
    {
        await _studentRepository.UpdateAsync(student);
    }

    public async Task DeleteStudentAsync(long id)
    {
        await _studentRepository.DeleteAsync(id);
    }
}
