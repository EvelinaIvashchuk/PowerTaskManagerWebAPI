using PowerTaskManager.Models;

namespace PowerTaskManager.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITaskItemRepository TaskItems { get; }
    ICategoryRepository Categories { get; }
    IUserRepository Users { get; }
    
    Task SaveAsync();
}