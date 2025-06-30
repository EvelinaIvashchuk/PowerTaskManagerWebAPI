using Microsoft.EntityFrameworkCore;
using PowerTaskManager.Data;
using PowerTaskManager.Models;
using PowerTaskManager.Repositories.Interfaces;

namespace PowerTaskManager.Repositories;

public class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
{
    public async Task<Category?> GetCategoryWithTasksAsync(int categoryId)
    {
        return await DbSet.Where(c => c.Id == categoryId)
            .Include(c => c.Tasks)
            .ThenInclude(t => t.User)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<Category>> GetCategoriesWithTasksAsync()
    {
        return await DbSet
            .Include(c => c.Tasks)
            .ThenInclude(t => t.User)
            .ToListAsync();
    }
}