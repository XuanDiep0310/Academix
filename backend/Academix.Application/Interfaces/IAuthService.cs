using Academix.Application.DTOs.Auth;
using Academix.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
    }
}
