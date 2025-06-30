using PowerTaskManager.DTOs;

namespace PowerTaskManager.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponseDto<PagedResponseDto<UserDto>>> GetAllUsersAsync(UserQueryParameters parameters);
    Task<ApiResponseDto<UserDto>> GetUserByIdAsync(string id);
    Task<ApiResponseDto<UserDto>> GetUserWithTasksAsync(string id);
    Task<ApiResponseDto<UserDto>> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
    Task<ApiResponseDto<bool>> DeleteUserAsync(string id);
}