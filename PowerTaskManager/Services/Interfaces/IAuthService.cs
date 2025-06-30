using PowerTaskManager.DTOs;

namespace PowerTaskManager.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    Task<ApiResponseDto<bool>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    Task<ApiResponseDto<bool>> LogoutAsync(string userId);
}