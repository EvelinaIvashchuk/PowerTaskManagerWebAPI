using PowerTaskManager.Models;

namespace PowerTaskManager.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category> GetCategoryWithTasksAsync(int categoryId);
    Task<IEnumerable<Category>> GetCategoriesWithTasksAsync();
}