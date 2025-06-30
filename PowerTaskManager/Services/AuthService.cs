using Microsoft.AspNetCore.Identity;
using PowerTaskManager.DTOs;
using PowerTaskManager.Helpers;
using PowerTaskManager.Models;
using PowerTaskManager.Services.Interfaces;

namespace PowerTaskManager.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtTokenGenerator _tokenGenerator;
    
    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        JwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
    }
    
    public async Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            
            if (existingUser != null)
            {
                return ApiResponseDto<AuthResponseDto>.Failure($"User with email '{registerDto.Email}' already exists");
            }
            
            // Create new user
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                EmailConfirmed = true // Auto-confirm email for simplicity
            };
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponseDto<AuthResponseDto>.Failure("Failed to register user", errors);
            }
            
            // Add user to 'User' role
            await _userManager.AddToRoleAsync(user, "User");
            
            // Generate token
            var (token, expiration) = await _tokenGenerator.GenerateTokenAsync(user);
            
            // Create response
            var authResponse = new AuthResponseDto
            {
                IsSuccess = true,
                Message = "User registered successfully",
                Token = token,
                RefreshToken = "", // Refresh token not implemented for simplicity
                Expiration = expiration,
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TaskCount = 0
                }
            };
            
            return ApiResponseDto<AuthResponseDto>.Success(authResponse);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<AuthResponseDto>.Failure($"Error registering user: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user == null)
            {
                return ApiResponseDto<AuthResponseDto>.Failure("Invalid email or password");
            }
            
            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            
            if (!result.Succeeded)
            {
                return ApiResponseDto<AuthResponseDto>.Failure("Invalid email or password");
            }
            
            // Generate token
            var (token, expiration) = await _tokenGenerator.GenerateTokenAsync(user);
            
            // Create response
            var authResponse = new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successful",
                Token = token,
                RefreshToken = "", // Refresh token not implemented for simplicity
                Expiration = expiration,
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TaskCount = user.Tasks.Count
                }
            };
            
            return ApiResponseDto<AuthResponseDto>.Success(authResponse);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<AuthResponseDto>.Failure($"Error logging in: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        // For simplicity, we'll just return a failure response
        // In a real application, you would validate the refresh token and generate a new access token
        return ApiResponseDto<AuthResponseDto>.Failure("Refresh token functionality not implemented");
    }
    
    public async Task<ApiResponseDto<bool>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return ApiResponseDto<bool>.Failure($"User with ID {userId} not found");
            }
            
            // Check current password
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
            
            if (!isCurrentPasswordValid)
            {
                return ApiResponseDto<bool>.Failure("Current password is incorrect");
            }
            
            // Change password
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponseDto<bool>.Failure("Failed to change password", errors);
            }
            
            return ApiResponseDto<bool>.Success(true, "Password changed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.Failure($"Error changing password: {ex.Message}");
        }
    }
    
    public async Task<ApiResponseDto<bool>> LogoutAsync(string userId)
    {
        try
        {
            // For simplicity, we'll just return a success response
            // In a real application with refresh tokens, you would invalidate the refresh token
            await _signInManager.SignOutAsync();
            return ApiResponseDto<bool>.Success(true, "Logout successful");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.Failure($"Error logging out: {ex.Message}");
        }
    }
}