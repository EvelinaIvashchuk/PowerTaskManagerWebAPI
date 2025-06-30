namespace PowerTaskManager.DTOs;

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Related entities
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class CreateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public int? CategoryId { get; set; }
}

public class UpdateTaskItemDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public int? CategoryId { get; set; }
}

public class TaskItemQueryParameters
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? UserId { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}