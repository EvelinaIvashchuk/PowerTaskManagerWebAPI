using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerTaskManager.DTOs;
using PowerTaskManager.Services.Interfaces;

namespace PowerTaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskItemController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;
    
    public TaskItemController(ITaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllTasks([FromQuery] TaskItemQueryParameters parameters)
    {
        var response = await _taskItemService.GetAllTasksAsync(parameters);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var response = await _taskItemService.GetTaskByIdAsync(id);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTasksByUserId(string userId)
    {
        var response = await _taskItemService.GetTasksByUserIdAsync(userId);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTasksByCategoryId(int categoryId)
    {
        var response = await _taskItemService.GetTasksByCategoryIdAsync(categoryId);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTasksByStatus(string status)
    {
        var response = await _taskItemService.GetTasksByStatusAsync(status);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("priority/{priority}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTasksByPriority(string priority)
    {
        var response = await _taskItemService.GetTasksByPriorityAsync(priority);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("due-today")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTasksDueToday()
    {
        var response = await _taskItemService.GetTasksDueTodayAsync();
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("overdue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTasksOverdue()
    {
        var response = await _taskItemService.GetTasksOverdueAsync();
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskItemDto createTaskDto)
    {
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiResponseDto<bool>
            {
                IsSuccess = false,
                Message = "User not authenticated"
            });
        }
        
        var response = await _taskItemService.CreateTaskAsync(createTaskDto, userId);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return CreatedAtAction(nameof(GetTaskById), new { id = response.Data.Id }, response);
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskItemDto updateTaskDto)
    {
        var response = await _taskItemService.UpdateTaskAsync(id, updateTaskDto);
        
        if (!response.IsSuccess)
        {
            if (response.Message.Contains("not found"))
            {
                return NotFound(response);
            }
            
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var response = await _taskItemService.DeleteTaskAsync(id);
        
        if (!response.IsSuccess)
        {
            if (response.Message.Contains("not found"))
            {
                return NotFound(response);
            }
            
            return BadRequest(response);
        }
        
        return Ok(response);
    }
}