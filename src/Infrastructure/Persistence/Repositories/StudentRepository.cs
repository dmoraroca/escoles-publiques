using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
/// <summary>
/// Centralizes persistent data access for student.
/// </summary>
public class StudentRepository : IStudentRepository
{
    private readonly SchoolDbContext _context;
    /// <summary>
    /// Initializes a new instance of the StudentRepository class with its required dependencies.
    /// </summary>
    public StudentRepository(SchoolDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Retrieves all async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.School)
            .Include(s => s.User)
            .ToListAsync();
    }
    /// <summary>
    /// Retrieves by id async and returns it to the caller.
    /// </summary>
    public async Task<Student?> GetByIdAsync(long id)
    {
        return await _context.Students
            .Include(s => s.School)
            .Include(s => s.User)
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    /// <summary>
    /// Retrieves by school id async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<Student>> GetBySchoolIdAsync(long schoolId)
    {
        return await _context.Students
            .Include(s => s.User)
            .Where(s => s.SchoolId == schoolId)
            .ToListAsync();
    }
    /// <summary>
    /// Executes the add async operation as part of this component.
    /// </summary>
    public async Task<Student> AddAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }
    /// <summary>
    /// Updates async with the data received in the request.
    /// </summary>
    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Deletes async from the system in a controlled manner.
    /// </summary>
    public async Task DeleteAsync(long id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
