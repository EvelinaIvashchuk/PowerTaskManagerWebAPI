using PowerTaskManager.Models;

namespace PowerTaskManager.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserWithTasksAsync(string userId);
    Task<IEnumerable<User>> GetUsersWithTasksAsync();
}