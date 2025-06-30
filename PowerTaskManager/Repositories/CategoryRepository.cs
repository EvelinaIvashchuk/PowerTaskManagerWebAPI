using Microsoft.EntityFrameworkCore;
using PowerTaskManager.Data;
using PowerTaskManager.Models;
using PowerTaskManager.Repositories.Interfaces;

namespace PowerTaskManager.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<Category> GetCategoryWithTasksAsync(int categoryId)
    {
        return await _dbSet.Where(c => c.Id == categoryId)
            .Include(c => c.Tasks)
            .ThenInclude(t => t.User)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<Category>> GetCategoriesWithTasksAsync()
    {
        return await _dbSet
            .Include(c => c.Tasks)
            .ThenInclude(t => t.User)
            .ToListAsync();
    }
}