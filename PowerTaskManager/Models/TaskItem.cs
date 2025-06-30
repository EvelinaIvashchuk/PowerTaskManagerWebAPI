using System.ComponentModel.DataAnnotations;

namespace PowerTaskManager.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign keys
    public string? UserId { get; set; }
    public int? CategoryId { get; set; }
    
    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Category? Category { get; set; }
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Urgent
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed,
    Cancelled
}