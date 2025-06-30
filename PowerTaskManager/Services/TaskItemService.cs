using System.Linq.Expressions;
using PowerTaskManager.DTOs;
using PowerTaskManager.Models;
using PowerTaskManager.Repositories.Interfaces;
using PowerTaskManager.Services.Interfaces;
using TaskStatus = PowerTaskManager.Models.TaskStatus;

namespace PowerTaskManager.Services;

public class TaskItemService : ITaskItemService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public TaskItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ApiResponseDto<PagedResponseDto<TaskItemDto>>> GetAllTasksAsync(TaskItemQueryParameters parameters)
    {
        try
        {
            // Build filter expression
            var filter = PredicateBuilder.True<TaskItem>();
            
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                filter = filter.And(t => t.Title.Contains(parameters.SearchTerm) || 
                                        t.Description.Contains(parameters.SearchTerm));
            }
            
            if (!string.IsNullOrEmpty(parameters.Status) && Enum.TryParse<TaskStatus>(parameters.Status, true, out var status))
            {
                filter = filter.And(t => t.Status == status);
            }
            
            if (!string.IsNullOrEmpty(parameters.Priority) && Enum.TryParse<TaskPriority>(parameters.Priority, true, out var priority))
            {
                filter = filter.And(t => t.Priority == priority);
            }
            
            if (!string.IsNullOrEmpty(parameters.UserId))
            {
                filter = filter.And(t => t.UserId == parameters.UserId);
            }
            
            if (parameters.CategoryId.HasValue)
            {
                filter = filter.And(t => t.CategoryId == parameters.CategoryId);
            }
            
            if (parameters.DueDateFrom.HasValue)
            {
                filter = filter.And(t => t.DueDate >= parameters.DueDateFrom.Value);
            }
            
            if (parameters.DueDateTo.HasValue)
            {
                filter = filter.And(t => t.DueDate <= parameters.DueDateTo.Value);
            }
            
            // Build order expression
            Func<IQueryable<TaskItem>, IOrderedQueryable<TaskItem>> orderBy = null;
            
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "title":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(t => t.Title) : q.OrderBy(t => t.Title);
                        break;
                    case "duedate":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(t => t.DueDate) : q.OrderBy(t => t.DueDate);
                        break;
                    case "priority":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(t => t.Priority) : q.OrderBy(t => t.Priority);
                        break;
                    case "status":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(t => t.Status) : q.OrderBy(t => t.Status);
                        break;
                    case "createdat":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(t => t.CreatedAt) : q.OrderBy(t => t.CreatedAt);
                        break;
                    default:
                        orderBy = q => q.OrderBy(t => t.DueDate);
                        break;
                }
            }
            else
            {
                orderBy = q => q.OrderBy(t => t.DueDate);
            }
            
            // Get total count
            var totalCount = await _unitOfWork.TaskItems.CountAsync(filter);
            
            // Get paginated data
            var tasks = await _unitOfWork.TaskItems.GetAsync(
                filter,
                orderBy,
                "User,Category",
                (parameters.PageNumber - 1) * parameters.PageSize,
                parameters.PageSize);
            
            // Map to DTOs
            var taskDtos = tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId,
                UserName = t.User?.UserName,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            }).ToList();
            
            // Create paged response
            var pagedResponse = new PagedResponseDto<TaskItemDto>
            {
                Items = taskDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
            };
            
            return ApiResponseDto<PagedResponseDto<TaskItemDto>>.Success(pagedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<PagedResponseDto<TaskItemDto>>.Failure($"Error retrieving tasks: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<TaskItemDto>> GetTaskByIdAsync(int id)
    {
        try
        {
            var task = await _unitOfWork.TaskItems.GetFirstOrDefaultAsync(t => t.Id == id, "User,Category");
            
            if (task == null)
            {
                return ApiResponseDto<TaskItemDto>.Failure($"Task with ID {id} not found");
            }
            
            var taskDto = new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                Status = task.Status.ToString(),
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                UserId = task.UserId,
                UserName = task.User?.UserName,
                CategoryId = task.CategoryId,
                CategoryName = task.Category?.Name
            };
            
            return ApiResponseDto<TaskItemDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<TaskItemDto>.Failure($"Error retrieving task: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksByUserIdAsync(string userId)
    {
        try
        {
            var tasks = await _unitOfWork.TaskItems.GetTasksByUserIdAsync(userId);
            
            var taskDtos = tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId,
                UserName = t.User?.UserName,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            }).ToList();
            
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Failure($"Error retrieving tasks: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksByCategoryIdAsync(int categoryId)
    {
        try
        {
            var tasks = await _unitOfWork.TaskItems.GetTasksByCategoryIdAsync(categoryId);
            
            var taskDtos = tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId,
                UserName = t.User?.UserName,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            }).ToList();
            
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Failure($"Error retrieving tasks: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksByStatusAsync(string status)
    {
        try
        {
            if (!Enum.TryParse<TaskStatus>(status, true, out var taskStatus))
            {
                return ApiResponseDto<IEnumerable<TaskItemDto>>.Failure($"Invalid status: {status}");
            }
            
            var tasks = await _unitOfWork.TaskItems.GetTasksByStatusAsync(taskStatus);
            
            var taskDtos = tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId,
                UserName = t.User?.UserName,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            }).ToList();
            
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Failure($"Error retrieving tasks: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksByPriorityAsync(string priority)
    {
        try
        {
            if (!Enum.TryParse<TaskPriority>(priority, true, out var taskPriority))
            {
                return ApiResponseDto<IEnumerable<TaskItemDto>>.Failure($"Invalid priority: {priority}");
            }
            
            var tasks = await _unitOfWork.TaskItems.GetTasksByPriorityAsync(taskPriority);
            
            var taskDtos = tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId,
                UserName = t.User?.UserName,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            }).ToList();
            
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Failure($"Error retrieving tasks: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksDueTodayAsync()
    {
        try
        {
            var tasks = await _unitOfWork.TaskItems.GetTasksDueTodayAsync();
            
            var taskDtos = tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId,
                UserName = t.User?.UserName,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            }).ToList();
            
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Failure($"Error retrieving tasks: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksOverdueAsync()
    {
        try
        {
            var tasks = await _unitOfWork.TaskItems.GetTasksOverdueAsync();
            
            var taskDtos = tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId,
                UserName = t.User?.UserName,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            }).ToList();
            
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<IEnumerable<TaskItemDto>>.Failure($"Error retrieving tasks: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<TaskItemDto>> CreateTaskAsync(CreateTaskItemDto createTaskDto, string userId)
    {
        try
        {
            // Validate priority
            if (!Enum.TryParse<TaskPriority>(createTaskDto.Priority, true, out var priority))
            {
                return ApiResponseDto<TaskItemDto>.Failure($"Invalid priority: {createTaskDto.Priority}");
            }
            
            // Create task
            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                Priority = priority,
                Status = TaskStatus.NotStarted,
                CreatedAt = DateTime.UtcNow,
                UserId = string.IsNullOrEmpty(createTaskDto.UserId) ? userId : createTaskDto.UserId,
                CategoryId = createTaskDto.CategoryId
            };
            
            await _unitOfWork.TaskItems.AddAsync(task);
            await _unitOfWork.SaveAsync();
            
            // Get the created task with related entities
            var createdTask = await _unitOfWork.TaskItems.GetFirstOrDefaultAsync(t => t.Id == task.Id, "User,Category");
            
            var taskDto = new TaskItemDto
            {
                Id = createdTask.Id,
                Title = createdTask.Title,
                Description = createdTask.Description,
                DueDate = createdTask.DueDate,
                Priority = createdTask.Priority.ToString(),
                Status = createdTask.Status.ToString(),
                CreatedAt = createdTask.CreatedAt,
                UpdatedAt = createdTask.UpdatedAt,
                UserId = createdTask.UserId,
                UserName = createdTask.User?.UserName,
                CategoryId = createdTask.CategoryId,
                CategoryName = createdTask.Category?.Name
            };
            
            return ApiResponseDto<TaskItemDto>.Success(taskDto, "Task created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<TaskItemDto>.Failure($"Error creating task: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<TaskItemDto>> UpdateTaskAsync(int id, UpdateTaskItemDto updateTaskDto)
    {
        try
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            
            if (task == null)
            {
                return ApiResponseDto<TaskItemDto>.Failure($"Task with ID {id} not found");
            }
            
            // Update properties if provided
            if (!string.IsNullOrEmpty(updateTaskDto.Title))
            {
                task.Title = updateTaskDto.Title;
            }
            
            if (!string.IsNullOrEmpty(updateTaskDto.Description))
            {
                task.Description = updateTaskDto.Description;
            }
            
            if (updateTaskDto.DueDate.HasValue)
            {
                task.DueDate = updateTaskDto.DueDate.Value;
            }
            
            if (!string.IsNullOrEmpty(updateTaskDto.Priority) && 
                Enum.TryParse<TaskPriority>(updateTaskDto.Priority, true, out var priority))
            {
                task.Priority = priority;
            }
            
            if (!string.IsNullOrEmpty(updateTaskDto.Status) && 
                Enum.TryParse<TaskStatus>(updateTaskDto.Status, true, out var status))
            {
                task.Status = status;
            }
            
            if (updateTaskDto.CategoryId.HasValue)
            {
                task.CategoryId = updateTaskDto.CategoryId;
            }
            
            task.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.TaskItems.Update(task);
            await _unitOfWork.SaveAsync();
            
            // Get the updated task with related entities
            var updatedTask = await _unitOfWork.TaskItems.GetFirstOrDefaultAsync(t => t.Id == id, "User,Category");
            
            var taskDto = new TaskItemDto
            {
                Id = updatedTask.Id,
                Title = updatedTask.Title,
                Description = updatedTask.Description,
                DueDate = updatedTask.DueDate,
                Priority = updatedTask.Priority.ToString(),
                Status = updatedTask.Status.ToString(),
                CreatedAt = updatedTask.CreatedAt,
                UpdatedAt = updatedTask.UpdatedAt,
                UserId = updatedTask.UserId,
                UserName = updatedTask.User?.UserName,
                CategoryId = updatedTask.CategoryId,
                CategoryName = updatedTask.Category?.Name
            };
            
            return ApiResponseDto<TaskItemDto>.Success(taskDto, "Task updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<TaskItemDto>.Failure($"Error updating task: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<bool>> DeleteTaskAsync(int id)
    {
        try
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            
            if (task == null)
            {
                return ApiResponseDto<bool>.Failure($"Task with ID {id} not found");
            }
            
            _unitOfWork.TaskItems.Remove(task);
            await _unitOfWork.SaveAsync();
            
            return ApiResponseDto<bool>.Success(true, "Task deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.Failure($"Error deleting task: {ex.Message}");
        }
    }
}

// Helper class for building predicates
public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() { return f => true; }
    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }
}