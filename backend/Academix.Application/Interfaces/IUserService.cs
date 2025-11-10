using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IUserService
    {
        // User CRUD Operations
        Task<ApiResponse<UserListResponseDto>> GetAllUsersAsync(
            string? role = null,
            bool? isActive = null,
            string? search = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc");

        Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(int userId);

        Task<ApiResponse<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request);

        Task<ApiResponse<BulkCreateUserResponseDto>> CreateUsersAsync(BulkCreateUserRequestDto request);

        Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int userId, UpdateUserRequestDto request);

        Task<ApiResponse<string>> DeleteUserAsync(int userId);

        Task<ApiResponse<string>> ToggleUserStatusAsync(int userId);

        // Profile Operations
        Task<ApiResponse<UserResponseDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);

        // Statistics
        Task<ApiResponse<UserStatisticsDto>> GetUserStatisticsAsync();

        // Validation
        Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);
        Task<bool> CanDeleteUserAsync(int userId);
    }
}
