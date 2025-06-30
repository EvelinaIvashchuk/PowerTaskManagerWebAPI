using Microsoft.EntityFrameworkCore;
using PowerTaskManager.Data;
using PowerTaskManager.Models;
using PowerTaskManager.Repositories.Interfaces;
using TaskStatus = PowerTaskManager.Models.TaskStatus;

namespace PowerTaskManager.Repositories;

public class TaskItemRepository(ApplicationDbContext context) : Repository<TaskItem>(context), ITaskItemRepository
{
    public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(string userId)
    {
        return await DbSet.Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TaskItem>> GetTasksByCategoryIdAsync(int categoryId)
    {
        return await DbSet.Where(t => t.CategoryId == categoryId)
            .Include(t => t.User)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status)
    {
        return await DbSet.Where(t => t.Status == status)
            .Include(t => t.User)
            .Include(t => t.Category)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskPriority priority)
    {
        return await DbSet.Where(t => t.Priority == priority)
            .Include(t => t.User)
            .Include(t => t.Category)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TaskItem>> GetTasksDueTodayAsync()
    {
        var today = DateTime.Today;
        return await DbSet.Where(t => t.DueDate.Date == today)
            .Include(t => t.User)
            .Include(t => t.Category)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TaskItem>> GetTasksOverdueAsync()
    {
        var today = DateTime.Today;
        return await DbSet.Where(t => t.DueDate.Date < today && t.Status != TaskStatus.Completed)
            .Include(t => t.User)
            .Include(t => t.Category)
            .ToListAsync();
    }
}