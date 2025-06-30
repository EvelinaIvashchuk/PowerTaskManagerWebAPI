using PowerTaskManager.DTOs;

namespace PowerTaskManager.Services.Interfaces;

public interface ICategoryService
{
    Task<ApiResponseDto<PagedResponseDto<CategoryDto>>> GetAllCategoriesAsync(CategoryQueryParameters parameters);
    Task<ApiResponseDto<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<ApiResponseDto<CategoryDto>> GetCategoryWithTasksAsync(int id);
    Task<ApiResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
    Task<ApiResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
    Task<ApiResponseDto<bool>> DeleteCategoryAsync(int id);
}