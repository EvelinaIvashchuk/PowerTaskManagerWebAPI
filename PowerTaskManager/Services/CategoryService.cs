using PowerTaskManager.DTOs;
using PowerTaskManager.Models;
using PowerTaskManager.Repositories.Interfaces;
using PowerTaskManager.Services.Interfaces;

namespace PowerTaskManager.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ApiResponseDto<PagedResponseDto<CategoryDto>>> GetAllCategoriesAsync(CategoryQueryParameters parameters)
    {
        try
        {
            // Build filter expression
            var filter = PredicateBuilder.True<Category>();
            
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                filter = filter.And(c => c.Name.Contains(parameters.SearchTerm) || 
                                        c.Description.Contains(parameters.SearchTerm));
            }
            
            // Build order expression
            Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null;
            
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "name":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(c => c.Name) : q.OrderBy(c => c.Name);
                        break;
                    case "taskcount":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(c => c.Tasks.Count) : q.OrderBy(c => c.Tasks.Count);
                        break;
                    default:
                        orderBy = q => q.OrderBy(c => c.Name);
                        break;
                }
            }
            else
            {
                orderBy = q => q.OrderBy(c => c.Name);
            }
            
            // Get total count
            var totalCount = await _unitOfWork.Categories.CountAsync(filter);
            
            // Get paginated data
            var categories = await _unitOfWork.Categories.GetAsync(
                filter,
                orderBy,
                "Tasks",
                (parameters.PageNumber - 1) * parameters.PageSize,
                parameters.PageSize);
            
            // Map to DTOs
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color,
                TaskCount = c.Tasks.Count
            }).ToList();
            
            // Create paged response
            var pagedResponse = new PagedResponseDto<CategoryDto>
            {
                Items = categoryDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
            };
            
            return ApiResponseDto<PagedResponseDto<CategoryDto>>.Success(pagedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<PagedResponseDto<CategoryDto>>.Failure($"Error retrieving categories: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Id == id, "Tasks");
            
            if (category == null)
            {
                return ApiResponseDto<CategoryDto>.Failure($"Category with ID {id} not found");
            }
            
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Color = category.Color,
                TaskCount = category.Tasks.Count
            };
            
            return ApiResponseDto<CategoryDto>.Success(categoryDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CategoryDto>.Failure($"Error retrieving category: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<CategoryDto>> GetCategoryWithTasksAsync(int id)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetCategoryWithTasksAsync(id);
            
            if (category == null)
            {
                return ApiResponseDto<CategoryDto>.Failure($"Category with ID {id} not found");
            }
            
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Color = category.Color,
                TaskCount = category.Tasks.Count
            };
            
            return ApiResponseDto<CategoryDto>.Success(categoryDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CategoryDto>.Failure($"Error retrieving category: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        try
        {
            // Check if category with the same name already exists
            var existingCategory = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Name == createCategoryDto.Name);
            
            if (existingCategory != null)
            {
                return ApiResponseDto<CategoryDto>.Failure($"Category with name '{createCategoryDto.Name}' already exists");
            }
            
            // Create category
            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                Color = createCategoryDto.Color
            };
            
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveAsync();
            
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Color = category.Color,
                TaskCount = 0
            };
            
            return ApiResponseDto<CategoryDto>.Success(categoryDto, "Category created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CategoryDto>.Failure($"Error creating category: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            
            if (category == null)
            {
                return ApiResponseDto<CategoryDto>.Failure($"Category with ID {id} not found");
            }
            
            // Check if name is being updated and if it already exists
            if (!string.IsNullOrEmpty(updateCategoryDto.Name) && updateCategoryDto.Name != category.Name)
            {
                var existingCategory = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Name == updateCategoryDto.Name);
                
                if (existingCategory != null)
                {
                    return ApiResponseDto<CategoryDto>.Failure($"Category with name '{updateCategoryDto.Name}' already exists");
                }
                
                category.Name = updateCategoryDto.Name;
            }
            
            // Update other properties if provided
            if (!string.IsNullOrEmpty(updateCategoryDto.Description))
            {
                category.Description = updateCategoryDto.Description;
            }
            
            if (!string.IsNullOrEmpty(updateCategoryDto.Color))
            {
                category.Color = updateCategoryDto.Color;
            }
            
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveAsync();
            
            // Get updated category with tasks
            var updatedCategory = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Id == id, "Tasks");
            
            var categoryDto = new CategoryDto
            {
                Id = updatedCategory.Id,
                Name = updatedCategory.Name,
                Description = updatedCategory.Description,
                Color = updatedCategory.Color,
                TaskCount = updatedCategory.Tasks.Count
            };
            
            return ApiResponseDto<CategoryDto>.Success(categoryDto, "Category updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CategoryDto>.Failure($"Error updating category: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<bool>> DeleteCategoryAsync(int id)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Id == id, "Tasks");
            
            if (category == null)
            {
                return ApiResponseDto<bool>.Failure($"Category with ID {id} not found");
            }
            
            // Check if category has tasks
            if (category.Tasks.Any())
            {
                return ApiResponseDto<bool>.Failure($"Cannot delete category with ID {id} because it has associated tasks");
            }
            
            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveAsync();
            
            return ApiResponseDto<bool>.Success(true, "Category deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.Failure($"Error deleting category: {ex.Message}");
        }
    }
}