using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerTaskManager.DTOs;
using PowerTaskManager.Services.Interfaces;

namespace PowerTaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllCategories([FromQuery] CategoryQueryParameters parameters)
    {
        var response = await _categoryService.GetAllCategoriesAsync(parameters);
        
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
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var response = await _categoryService.GetCategoryByIdAsync(id);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    [HttpGet("{id:int}/tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategoryWithTasks(int id)
    {
        var response = await _categoryService.GetCategoryWithTasksAsync(id);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        var response = await _categoryService.CreateCategoryAsync(createCategoryDto);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        
        return CreatedAtAction(nameof(GetCategoryById), new { id = response.Data.Id }, response);
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        var response = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
        
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var response = await _categoryService.DeleteCategoryAsync(id);
        
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