using Domain.Entities;

namespace Domain.Interfaces;
/// <summary>
/// Centralizes persistent data access for i enrollment.
/// </summary>
public interface IEnrollmentRepository
{
    Task<IEnumerable<Enrollment>> GetAllAsync();
    Task<Enrollment?> GetByIdAsync(long id);
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(long studentId);
    Task<Enrollment> AddAsync(Enrollment enrollment);
    Task UpdateAsync(Enrollment enrollment);
    Task DeleteAsync(long id);
}
