using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
/// <summary>
/// Centralizes persistent data access for user.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly SchoolDbContext _context;
            /// <summary>
            /// Initializes a new instance of the UserRepository class with its required dependencies.
            /// </summary>
            public UserRepository(SchoolDbContext context)
    {
        _context = context;
    }
            /// <summary>
            /// Retrieves by id async and returns it to the caller.
            /// </summary>
            public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
            /// <summary>
            /// Retrieves by email async and returns it to the caller.
            /// </summary>
            public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }
            /// <summary>
            /// Retrieves all async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Student)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();
    }
            /// <summary>
            /// Executes the add async operation as part of this component.
            /// </summary>
            public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
            /// <summary>
            /// Updates async with the data received in the request.
            /// </summary>
            public async Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
            /// <summary>
            /// Deletes async from the system in a controlled manner.
            /// </summary>
            public async Task DeleteAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
            /// <summary>
            /// Executes the email exists async operation as part of this component.
            /// </summary>
            public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
