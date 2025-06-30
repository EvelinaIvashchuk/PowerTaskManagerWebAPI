using PowerTaskManager.Data;
using PowerTaskManager.Repositories.Interfaces;

namespace PowerTaskManager.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    ITaskItemRepository taskItems,
    ICategoryRepository categories,
    IUserRepository users)
    : IUnitOfWork
{
    public ITaskItemRepository TaskItems { get; } = taskItems;

    public ICategoryRepository Categories { get; } = categories;

    public IUserRepository Users { get; } = users;

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}