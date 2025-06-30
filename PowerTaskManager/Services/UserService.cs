using Microsoft.AspNetCore.Identity;
using PowerTaskManager.DTOs;
using PowerTaskManager.Models;
using PowerTaskManager.Repositories.Interfaces;
using PowerTaskManager.Services.Interfaces;

namespace PowerTaskManager.Services;

public class UserService(IUnitOfWork unitOfWork, UserManager<User> userManager) : IUserService
{
    public async Task<ApiResponseDto<PagedResponseDto<UserDto>>> GetAllUsersAsync(UserQueryParameters parameters)
    {
        try
        {
            // Build filter expression
            var filter = PredicateBuilder.True<User>();
            
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                filter = filter.And(u => u.UserName.Contains(parameters.SearchTerm) || 
                                        u.Email.Contains(parameters.SearchTerm) ||
                                        u.FirstName.Contains(parameters.SearchTerm) ||
                                        u.LastName.Contains(parameters.SearchTerm));
            }
            
            // Build order expression
            Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null;
            
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "username":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(u => u.UserName) : q.OrderBy(u => u.UserName);
                        break;
                    case "email":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(u => u.Email) : q.OrderBy(u => u.Email);
                        break;
                    case "firstname":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(u => u.FirstName) : q.OrderBy(u => u.FirstName);
                        break;
                    case "lastname":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(u => u.LastName) : q.OrderBy(u => u.LastName);
                        break;
                    case "taskcount":
                        orderBy = q => parameters.SortDescending ? q.OrderByDescending(u => u.Tasks.Count) : q.OrderBy(u => u.Tasks.Count);
                        break;
                    default:
                        orderBy = q => q.OrderBy(u => u.UserName);
                        break;
                }
            }
            else
            {
                orderBy = q => q.OrderBy(u => u.UserName);
            }
            
            // Get total count
            var totalCount = await unitOfWork.Users.CountAsync(filter);
            
            // Get paginated data
            var users = await unitOfWork.Users.GetAsync(
                filter,
                orderBy,
                "Tasks",
                (parameters.PageNumber - 1) * parameters.PageSize,
                parameters.PageSize);
            
            // Map to DTOs
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                TaskCount = u.Tasks.Count
            }).ToList();
            
            // Create paged response
            var pagedResponse = new PagedResponseDto<UserDto>
            {
                Items = userDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
            };
            
            return ApiResponseDto<PagedResponseDto<UserDto>>.Success(pagedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<PagedResponseDto<UserDto>>.Failure($"Error retrieving users: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<UserDto>> GetUserByIdAsync(string id)
    {
        try
        {
            var user = await unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Id == id, "Tasks");
            
            if (user == null)
            {
                return ApiResponseDto<UserDto>.Failure($"User with ID {id} not found");
            }
            
            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TaskCount = user.Tasks.Count
            };
            
            return ApiResponseDto<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<UserDto>.Failure($"Error retrieving user: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<UserDto>> GetUserWithTasksAsync(string id)
    {
        try
        {
            var user = await unitOfWork.Users.GetUserWithTasksAsync(id);
            
            if (user == null)
            {
                return ApiResponseDto<UserDto>.Failure($"User with ID {id} not found");
            }
            
            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TaskCount = user.Tasks.Count
            };
            
            return ApiResponseDto<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<UserDto>.Failure($"Error retrieving user: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<UserDto>> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return ApiResponseDto<UserDto>.Failure($"User with ID {id} not found");
            }
            
            // Update properties if provided
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
            {
                user.FirstName = updateUserDto.FirstName;
            }
            
            if (!string.IsNullOrEmpty(updateUserDto.LastName))
            {
                user.LastName = updateUserDto.LastName;
            }
            
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                // Check if email is already in use
                var existingUser = await userManager.FindByEmailAsync(updateUserDto.Email);
                
                if (existingUser != null)
                {
                    return ApiResponseDto<UserDto>.Failure($"Email '{updateUserDto.Email}' is already in use");
                }
                
                user.Email = updateUserDto.Email;
                user.UserName = updateUserDto.Email; // Assuming username is the same as email
                user.NormalizedEmail = updateUserDto.Email.ToUpper();
                user.NormalizedUserName = updateUserDto.Email.ToUpper();
            }
            
            var result = await userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponseDto<UserDto>.Failure("Failed to update user", errors);
            }
            
            // Get updated user with tasks
            var updatedUser = await unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Id == id, "Tasks");
            
            var userDto = new UserDto
            {
                Id = updatedUser.Id,
                UserName = updatedUser.UserName,
                Email = updatedUser.Email,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                TaskCount = updatedUser.Tasks.Count
            };
            
            return ApiResponseDto<UserDto>.Success(userDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<UserDto>.Failure($"Error updating user: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<bool>> DeleteUserAsync(string id)
    {
        try
        {
            var user = await userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return ApiResponseDto<bool>.Failure($"User with ID {id} not found");
            }
            
            // Check if user has tasks
            var userWithTasks = await unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Id == id, "Tasks");
            
            if (userWithTasks.Tasks.Any())
            {
                return ApiResponseDto<bool>.Failure($"Cannot delete user with ID {id} because they have associated tasks");
            }
            
            var result = await userManager.DeleteAsync(user);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponseDto<bool>.Failure("Failed to delete user", errors);
            }
            
            return ApiResponseDto<bool>.Success(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.Failure($"Error deleting user: {ex.Message}");
        }
    }
}