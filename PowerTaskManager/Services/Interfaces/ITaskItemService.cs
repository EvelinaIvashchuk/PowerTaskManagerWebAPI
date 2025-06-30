using PowerTaskManager.DTOs;
using PowerTaskManager.Models;

namespace PowerTaskManager.Services.Interfaces;

public interface ITaskItemService
{
    Task<ApiResponseDto<PagedResponseDto<TaskItemDto>>> GetAllTasksAsync(TaskItemQueryParameters parameters);
    Task<ApiResponseDto<TaskItemDto>> GetTaskByIdAsync(int id);
    Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksByUserIdAsync(string userId);
    Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksByCategoryIdAsync(int categoryId);
    Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksByStatusAsync(string status);
    Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksByPriorityAsync(string priority);
    Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksDueTodayAsync();
    Task<ApiResponseDto<IEnumerable<TaskItemDto>>> GetTasksOverdueAsync();
    Task<ApiResponseDto<TaskItemDto>> CreateTaskAsync(CreateTaskItemDto createTaskDto, string userId);
    Task<ApiResponseDto<TaskItemDto>> UpdateTaskAsync(int id, UpdateTaskItemDto updateTaskDto);
    Task<ApiResponseDto<bool>> DeleteTaskAsync(int id);
}