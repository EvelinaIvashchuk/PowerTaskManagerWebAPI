using PowerTaskManager.Models;
using TaskStatus = PowerTaskManager.Models.TaskStatus;

namespace PowerTaskManager.Repositories.Interfaces;

public interface ITaskItemRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(string userId);
    Task<IEnumerable<TaskItem>> GetTasksByCategoryIdAsync(int categoryId);
    Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status);
    Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskPriority priority);
    Task<IEnumerable<TaskItem>> GetTasksDueTodayAsync();
    Task<IEnumerable<TaskItem>> GetTasksOverdueAsync();
}