using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Interfície de servei per gestionar inscripcions a l'aplicació.
/// </summary>
public interface IEnrollmentService
{
    Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync();
    Task<Enrollment?> GetEnrollmentByIdAsync(long id);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(long studentId);
    Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
    Task UpdateEnrollmentAsync(Enrollment enrollment);
    Task DeleteEnrollmentAsync(long id);
}
