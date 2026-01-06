using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.DomainExceptions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

/// <summary>
/// Service for managing students, including retrieval, creation, update, and deletion of student records.
/// </summary>
public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly ILogger<StudentService> _logger;

    /// <summary>
    /// Initializes a new instance of the StudentService class.
    /// </summary>
    /// <param name="studentRepository">Repository for student data access.</param>
    /// <param name="schoolRepository">Repository for school data access.</param>
    /// <param name="logger">Logger instance.</param>
    public StudentService(
        IStudentRepository studentRepository,
        ISchoolRepository schoolRepository,
        ILogger<StudentService> logger)
    {
        _studentRepository = studentRepository;
        _schoolRepository = schoolRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all students asynchronously.
    /// </summary>
    /// <returns>Enumerable of all students.</returns>
    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        _logger.LogInformation("Obtenint tots els alumnes");
        return await _studentRepository.GetAllAsync();
    }

    /// <summary>
    /// Retrieves a student by their unique identifier asynchronously.
    /// </summary>
    /// <param name="id">Student identifier.</param>
    /// <returns>The student if found; otherwise, throws NotFoundException.</returns>
    public async Task<Student?> GetStudentByIdAsync(long id)
    {
        _logger.LogInformation("Obtenint alumne amb Id: {Id}", id);
        var student = await _studentRepository.GetByIdAsync(id);
        
        if (student == null)
        {
            _logger.LogWarning("Alumne amb Id {Id} no trobat", id);
            throw new NotFoundException("Student", id);
        }
        
        return student;
    }

    public async Task<IEnumerable<Student>> GetStudentsBySchoolIdAsync(long schoolId)
    {
        _logger.LogInformation("Obtenint alumnes de l'escola amb Id: {SchoolId}", schoolId);
        return await _studentRepository.GetBySchoolIdAsync(schoolId);
    }

    public async Task<Student> CreateStudentAsync(Student student)
    {
        // Validacions ara es fan a nivell d'User
        if (student.UserId.HasValue && student.User != null)
        {
            if (string.IsNullOrWhiteSpace(student.User.FirstName))
            {
                throw new ValidationException("FirstName", "El nom de l'alumne és obligatori");
            }
            
            if (string.IsNullOrWhiteSpace(student.User.LastName))
            {
                throw new ValidationException("LastName", "Els cognoms de l'alumne són obligatoris");
            }
        }
        
        if (student.SchoolId > 0)
        {
            var school = await _schoolRepository.GetByIdAsync(student.SchoolId);
            if (school == null)
            {
                throw new NotFoundException("School", student.SchoolId);
            }
        }

        student.CreatedAt = DateTime.UtcNow;
        _logger.LogInformation("Creant nou alumne per l'usuari: {UserId}", student.UserId);
        return await _studentRepository.AddAsync(student);
    }

    public async Task UpdateStudentAsync(Student student)
    {
        var existingStudent = await _studentRepository.GetByIdAsync(student.Id);
        if (existingStudent == null)
        {
            throw new NotFoundException("Student", student.Id);
        }
        
        _logger.LogInformation("Actualitzant alumne amb Id: {Id}", student.Id);
        await _studentRepository.UpdateAsync(student);
    }

    public async Task DeleteStudentAsync(long id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
        {
            throw new NotFoundException("Student", id);
        }
        
        _logger.LogInformation("Eliminant alumne amb Id: {Id}", id);
        await _studentRepository.DeleteAsync(id);
    }
}
