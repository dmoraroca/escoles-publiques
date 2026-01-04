using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(IEnrollmentRepository enrollmentRepository, ILogger<EnrollmentService> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
    {
        try
        {
            return await _enrollmentRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtenir totes les inscripcions");
            throw;
        }
    }

    public async Task<Enrollment?> GetEnrollmentByIdAsync(long id)
    {
        return await _enrollmentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(long studentId)
    {
        return await _enrollmentRepository.GetByStudentIdAsync(studentId);
    }

    public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
    {
        enrollment.EnrolledAt = DateTime.UtcNow;
        return await _enrollmentRepository.AddAsync(enrollment);
    }

    public async Task UpdateEnrollmentAsync(Enrollment enrollment)
    {
        await _enrollmentRepository.UpdateAsync(enrollment);
    }

    public async Task DeleteEnrollmentAsync(long id)
    {
        await _enrollmentRepository.DeleteAsync(id);
    }
}
