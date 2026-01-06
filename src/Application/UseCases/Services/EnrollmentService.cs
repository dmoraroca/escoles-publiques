using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.DomainExceptions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

/// <summary>
/// Service for managing enrollments, including retrieval, creation, update, and deletion of enrollment records.
/// </summary>
public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<EnrollmentService> _logger;

    /// <summary>
    /// Initializes a new instance of the EnrollmentService class.
    /// </summary>
    /// <param name="enrollmentRepository">Repository for enrollment data access.</param>
    /// <param name="studentRepository">Repository for student data access.</param>
    /// <param name="logger">Logger instance.</param>
    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ILogger<EnrollmentService> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all enrollments asynchronously.
    /// </summary>
    /// <returns>Enumerable of all enrollments.</returns>
    public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
    {
        _logger.LogInformation("Obtenint totes les inscripcions");
        return await _enrollmentRepository.GetAllAsync();
    }

    /// <summary>
    /// Retrieves an enrollment by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">Enrollment identifier.</param>
    /// <returns>The enrollment if found; otherwise, throws NotFoundException.</returns>
    public async Task<Enrollment?> GetEnrollmentByIdAsync(long id)
    {
        _logger.LogInformation("Obtenint inscripció amb Id: {Id}", id);
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);
        
        if (enrollment == null)
        {
            _logger.LogWarning("Inscripció amb Id {Id} no trobada", id);
            throw new NotFoundException("Enrollment", id);
        }
        
        return enrollment;
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(long studentId)
    {
        _logger.LogInformation("Obtenint inscripcions de l'alumne amb Id: {StudentId}", studentId);
        return await _enrollmentRepository.GetByStudentIdAsync(studentId);
    }

    public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
    {
        if (string.IsNullOrWhiteSpace(enrollment.AcademicYear))
        {
            throw new ValidationException("AcademicYear", "El curs acadèmic és obligatori");
        }
        
        if (enrollment.StudentId <= 0)
        {
            throw new ValidationException("StudentId", "L'ID de l'alumne és obligatori");
        }
        
        var student = await _studentRepository.GetByIdAsync(enrollment.StudentId);
        if (student == null)
        {
            throw new NotFoundException("Student", enrollment.StudentId);
        }

        enrollment.EnrolledAt = DateTime.UtcNow;
        _logger.LogInformation("Creant nova inscripció per l'alumne {StudentId} - Curs {AcademicYear}", 
            enrollment.StudentId, enrollment.AcademicYear);
        return await _enrollmentRepository.AddAsync(enrollment);
    }

    public async Task UpdateEnrollmentAsync(Enrollment enrollment)
    {
        var existingEnrollment = await _enrollmentRepository.GetByIdAsync(enrollment.Id);
        if (existingEnrollment == null)
        {
            throw new NotFoundException("Enrollment", enrollment.Id);
        }
        
        _logger.LogInformation("Actualitzant inscripció amb Id: {Id}", enrollment.Id);
        await _enrollmentRepository.UpdateAsync(enrollment);
    }

    public async Task DeleteEnrollmentAsync(long id)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);
        if (enrollment == null)
        {
            throw new NotFoundException("Enrollment", id);
        }
        
        _logger.LogInformation("Eliminant inscripció amb Id: {Id}", id);
        await _enrollmentRepository.DeleteAsync(id);
    }
}
