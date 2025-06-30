using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerTaskManager.DTOs;
using PowerTaskManager.Services.Interfaces;

namespace PowerTaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = "Admin")] // Only admins can see all users
    public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParameters parameters)
    {
        var response = await userService.GetAllUsersAsync(parameters);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserById(string id)
    {
        // Check if user is requesting their own data or is an admin
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        var isAdmin = User.IsInRole("Admin");
        
        if (userId != id && !isAdmin)
        {
            return Forbid();
        }
        
        var response = await userService.GetUserByIdAsync(id);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("{id}/tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserWithTasks(string id)
    {
        // Check if user is requesting their own data or is an admin
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        var isAdmin = User.IsInRole("Admin");
        
        if (userId != id && !isAdmin)
        {
            return Forbid();
        }
        
        var response = await userService.GetUserWithTasksAsync(id);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
    {
        // Check if user is updating their own data or is an admin
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        var isAdmin = User.IsInRole("Admin");
        
        if (userId != id && !isAdmin)
        {
            return Forbid();
        }
        
        var response = await userService.UpdateUserAsync(id, updateUserDto);
        
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
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        // Check if user is deleting their own account or is an admin
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        var isAdmin = User.IsInRole("Admin");
        
        if (userId != id && !isAdmin)
        {
            return Forbid();
        }
        
        var response = await userService.DeleteUserAsync(id);
        
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