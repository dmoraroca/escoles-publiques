using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.DomainExceptions;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Application.UseCases.Services;
/// <summary>
/// Implements application logic for student operations.
/// </summary>
public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IUserService _userService;
    private readonly ILogger<StudentService> _logger;

    public StudentService(
    IStudentRepository studentRepository,
    ISchoolRepository schoolRepository,
    IUserService userService,
    ILogger<StudentService> logger)
    {
        _studentRepository = studentRepository;
        _schoolRepository = schoolRepository;
        _userService = userService;
        _logger = logger;
    }
    /// <summary>
    /// Retrieves all students async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        _logger.LogInformation("Obtenint tots els alumnes");
        return await _studentRepository.GetAllAsync();
    }
    /// <summary>
    /// Retrieves student by id async and returns it to the caller.
    /// </summary>
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
    /// <summary>
    /// Retrieves students by school id async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<Student>> GetStudentsBySchoolIdAsync(long schoolId)
    {
        _logger.LogInformation("Obtenint alumnes de l'escola amb Id: {SchoolId}", schoolId);
        return await _studentRepository.GetBySchoolIdAsync(schoolId);
    }
    /// <summary>
    /// Creates student async by applying the required business rules.
    /// </summary>
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
    /// <summary>
    /// Creates student with user async by applying the required business rules.
    /// </summary>
    public async Task<Student> CreateStudentWithUserAsync(Domain.Entities.User user, string password, Student student)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // Create user (will throw if duplicate)
        var createdUser = await _userService.CreateUserAsync(user, password);

        // Associate and create student
        student.UserId = createdUser.Id;
        student.CreatedAt = DateTime.UtcNow;
        var createdStudent = await _studentRepository.AddAsync(student);

        scope.Complete();
        return createdStudent;
    }
    /// <summary>
    /// Updates student async with the data received in the request.
    /// </summary>
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
    /// <summary>
    /// Updates student with user async with the data received in the request.
    /// </summary>
    public async Task UpdateStudentWithUserAsync(Student student, Domain.Entities.User user)
    {
        var existingStudent = await _studentRepository.GetByIdAsync(student.Id);
        if (existingStudent == null)
        {
            throw new NotFoundException("Student", student.Id);
        }

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // Update user first
        await _userService.UpdateUserAsync(user);

        // Update student
        await _studentRepository.UpdateAsync(student);

        scope.Complete();
    }
    /// <summary>
    /// Deletes student async from the system in a controlled manner.
    /// </summary>
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
