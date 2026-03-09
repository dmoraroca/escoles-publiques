using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
/// <summary>
/// Centralizes persistent data access for enrollment.
/// </summary>
public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly SchoolDbContext _context;
    /// <summary>
    /// Initializes a new instance of the EnrollmentRepository class with its required dependencies.
    /// </summary>
    public EnrollmentRepository(SchoolDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Retrieves all async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments
            .Include(e => e.Student)
                .ThenInclude(s => s.User)
            .Include(e => e.School)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }
    /// <summary>
    /// Retrieves by id async and returns it to the caller.
    /// </summary>
    public async Task<Enrollment?> GetByIdAsync(long id)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
                .ThenInclude(s => s.User)
            .Include(e => e.AnnualFees)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    /// <summary>
    /// Retrieves by student id async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(long studentId)
    {
        return await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }
    /// <summary>
    /// Executes the add async operation as part of this component.
    /// </summary>
    public async Task<Enrollment> AddAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }
    /// <summary>
    /// Updates async with the data received in the request.
    /// </summary>
    public async Task UpdateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Deletes async from the system in a controlled manner.
    /// </summary>
    public async Task DeleteAsync(long id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}
