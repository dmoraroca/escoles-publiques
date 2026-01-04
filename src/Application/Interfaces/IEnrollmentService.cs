using Domain.Entities;

namespace Application.Interfaces;

public interface IEnrollmentService
{
    Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync();
    Task<Enrollment?> GetEnrollmentByIdAsync(long id);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(long studentId);
    Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
    Task UpdateEnrollmentAsync(Enrollment enrollment);
    Task DeleteEnrollmentAsync(long id);
}
