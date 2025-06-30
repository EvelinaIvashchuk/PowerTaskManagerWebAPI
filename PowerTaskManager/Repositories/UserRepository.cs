using Microsoft.EntityFrameworkCore;
using PowerTaskManager.Data;
using PowerTaskManager.Models;
using PowerTaskManager.Repositories.Interfaces;

namespace PowerTaskManager.Repositories;

public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetUserWithTasksAsync(string userId)
    {
        return await DbSet.Where(u => u.Id == userId)
            .Include(u => u.Tasks)
            .ThenInclude(t => t.Category)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<User>> GetUsersWithTasksAsync()
    {
        return await DbSet
            .Include(u => u.Tasks)
            .ThenInclude(t => t.Category)
            .ToListAsync();
    }
}