using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Users;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AcademixDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            AcademixDbContext context,
            IEmailService emailService,
            ILogger<UserService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ApiResponse<UserListResponseDto>> GetAllUsersAsync(
            string? role = null,
            bool? isActive = null,
            string? search = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            try
            {
                var query = _context.Users.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(role))
                {
                    query = query.Where(u => u.Role == role);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == isActive.Value);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(u =>
                        u.Email.ToLower().Contains(searchLower) ||
                        u.FullName.ToLower().Contains(searchLower));
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply sorting
                query = sortBy.ToLower() switch
                {
                    "email" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(u => u.Email)
                        : query.OrderByDescending(u => u.Email),
                    "fullname" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(u => u.FullName)
                        : query.OrderByDescending(u => u.FullName),
                    "role" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(u => u.Role)
                        : query.OrderByDescending(u => u.Role),
                    _ => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(u => u.CreatedAt)
                        : query.OrderByDescending(u => u.CreatedAt)
                };

                // Apply pagination
                var users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        Email = u.Email,
                        FullName = u.FullName,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    })
                    .ToListAsync();

                var response = new UserListResponseDto
                {
                    Users = users,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<UserListResponseDto>.SuccessResponse(response, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting users: {ex.Message}");
                return ApiResponse<UserListResponseDto>.ErrorResponse("Failed to retrieve users");
            }
        }

        public async Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return ApiResponse<UserResponseDto>.ErrorResponse("User not found");
                }

                var response = new UserResponseDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return ApiResponse<UserResponseDto>.SuccessResponse(response, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user {userId}: {ex.Message}");
                return ApiResponse<UserResponseDto>.ErrorResponse("Failed to retrieve user");
            }
        }

        public async Task<ApiResponse<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request)
        {
            try
            {
                // Validate role
                if (!UserRoles.IsValid(request.Role) || request.Role == UserRoles.Admin)
                {
                    return ApiResponse<UserResponseDto>.ErrorResponse("Invalid role");
                }

                // Check if email exists
                if (await IsEmailExistsAsync(request.Email))
                {
                    return ApiResponse<UserResponseDto>.ErrorResponse("Email already exists");
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create user
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FullName = request.FullName,
                    Role = request.Role,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Send welcome email
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName, request.Password);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to send welcome email to {user.Email}: {ex.Message}");
                }

                var response = new UserResponseDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                _logger.LogInformation($"User {user.Email} created successfully");
                return ApiResponse<UserResponseDto>.SuccessResponse(response, "User created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user: {ex.Message}");
                return ApiResponse<UserResponseDto>.ErrorResponse("Failed to create user");
            }
        }

        public async Task<ApiResponse<BulkCreateUserResponseDto>> CreateUsersAsync(BulkCreateUserRequestDto request)
        {
            var response = new BulkCreateUserResponseDto
            {
                TotalProcessed = request.Users.Count
            };

            foreach (var userRequest in request.Users)
            {
                try
                {
                    // Validate role
                    if (!UserRoles.IsValid(userRequest.Role) || userRequest.Role == UserRoles.Admin)
                    {
                        response.FailedUsers.Add(new UserCreationFailedDto
                        {
                            Email = userRequest.Email,
                            FullName = userRequest.FullName,
                            Reason = "Invalid role"
                        });
                        continue;
                    }

                    // Check if email exists
                    if (await IsEmailExistsAsync(userRequest.Email))
                    {
                        response.FailedUsers.Add(new UserCreationFailedDto
                        {
                            Email = userRequest.Email,
                            FullName = userRequest.FullName,
                            Reason = "Email already exists"
                        });
                        continue;
                    }

                    // Generate random password if not provided
                    var password = string.IsNullOrEmpty(userRequest.Password)
                        ? GenerateRandomPassword()
                        : userRequest.Password;

                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                    var user = new User
                    {
                        Email = userRequest.Email,
                        PasswordHash = passwordHash,
                        FullName = userRequest.FullName,
                        Role = userRequest.Role,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    response.SuccessfulUsers.Add(new UserCreatedDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role,
                        TemporaryPassword = password
                    });

                    // Send welcome email (async, don't wait)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName, password);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Failed to send welcome email to {user.Email}: {ex.Message}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating user {userRequest.Email}: {ex.Message}");
                    response.FailedUsers.Add(new UserCreationFailedDto
                    {
                        Email = userRequest.Email,
                        FullName = userRequest.FullName,
                        Reason = "Internal error"
                    });
                }
            }

            response.SuccessCount = response.SuccessfulUsers.Count;
            response.FailedCount = response.FailedUsers.Count;

            _logger.LogInformation($"Bulk creation completed: {response.SuccessCount} succeeded, {response.FailedCount} failed");

            return ApiResponse<BulkCreateUserResponseDto>.SuccessResponse(
                response,
                $"Processed {response.TotalProcessed} users: {response.SuccessCount} created, {response.FailedCount} failed");
        }

        public async Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int userId, UpdateUserRequestDto request)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return ApiResponse<UserResponseDto>.ErrorResponse("User not found");
                }

                // Check if trying to update admin
                if (user.Role == UserRoles.Admin)
                {
                    return ApiResponse<UserResponseDto>.ErrorResponse("Cannot update admin user");
                }

                // Update email if provided
                if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
                {
                    if (await IsEmailExistsAsync(request.Email, userId))
                    {
                        return ApiResponse<UserResponseDto>.ErrorResponse("Email already exists");
                    }
                    user.Email = request.Email;
                }

                // Update full name if provided
                if (!string.IsNullOrEmpty(request.FullName))
                {
                    user.FullName = request.FullName;
                }

                // Update active status if provided
                if (request.IsActive.HasValue)
                {
                    user.IsActive = request.IsActive.Value;
                }

                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new UserResponseDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                _logger.LogInformation($"User {userId} updated successfully");
                return ApiResponse<UserResponseDto>.SuccessResponse(response, "User updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user {userId}: {ex.Message}");
                return ApiResponse<UserResponseDto>.ErrorResponse("Failed to update user");
            }
        }

        public async Task<ApiResponse<string>> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return ApiResponse<string>.ErrorResponse("User not found");
                }

                // Cannot delete admin
                if (user.Role == UserRoles.Admin)
                {
                    return ApiResponse<string>.ErrorResponse("Cannot delete admin user");
                }

                // Check if user can be deleted
                if (!await CanDeleteUserAsync(userId))
                {
                    return ApiResponse<string>.ErrorResponse("Cannot delete user with existing data. Consider deactivating instead.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {userId} deleted successfully");
                return ApiResponse<string>.SuccessResponse("User deleted successfully", "User deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user {userId}: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to delete user");
            }
        }

        public async Task<ApiResponse<string>> ToggleUserStatusAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return ApiResponse<string>.ErrorResponse("User not found");
                }

                // Cannot toggle admin status
                if (user.Role == UserRoles.Admin)
                {
                    return ApiResponse<string>.ErrorResponse("Cannot change admin user status");
                }

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var status = user.IsActive ? "activated" : "deactivated";
                _logger.LogInformation($"User {userId} {status} successfully");

                return ApiResponse<string>.SuccessResponse(
                    $"User {status} successfully",
                    $"User {status} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error toggling user status {userId}: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to toggle user status");
            }
        }

        public async Task<ApiResponse<UserResponseDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto request)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return ApiResponse<UserResponseDto>.ErrorResponse("User not found");
                }

                user.FullName = request.FullName;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new UserResponseDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return ApiResponse<UserResponseDto>.SuccessResponse(response, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile {userId}: {ex.Message}");
                return ApiResponse<UserResponseDto>.ErrorResponse("Failed to update profile");
            }
        }

        public async Task<ApiResponse<UserStatisticsDto>> GetUserStatisticsAsync()
        {
            try
            {
                // OPTIMIZATION: Get all counts in one database query
                var countStats = await _context.Users
                    .GroupBy(u => 1) // Group all users into a single group
                    .Select(g => new
                    {
                        TotalUsers = g.Count(),
                        TotalAdmins = g.Count(u => u.Role == UserRoles.Admin),
                        TotalTeachers = g.Count(u => u.Role == UserRoles.Teacher),
                        TotalStudents = g.Count(u => u.Role == UserRoles.Student),
                        ActiveUsers = g.Count(u => u.IsActive),
                        InactiveUsers = g.Count(u => !u.IsActive)
                    })
                    .FirstOrDefaultAsync(); // Get the single aggregated result

                // Handle case where there are no users at all
                var statistics = new UserStatisticsDto();
                if (countStats != null)
                {
                    statistics.TotalUsers = countStats.TotalUsers;
                    statistics.TotalAdmins = countStats.TotalAdmins;
                    statistics.TotalTeachers = countStats.TotalTeachers;
                    statistics.TotalStudents = countStats.TotalStudents;
                    statistics.ActiveUsers = countStats.ActiveUsers;
                    statistics.InactiveUsers = countStats.InactiveUsers;
                }

                // FIXED QUERY: Get user growth for last 12 months
                var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);

                // 1. Let the database do the grouping, counting, and ordering
                var rawGrowthData = await _context.Users
                    .Where(u => u.CreatedAt >= twelveMonthsAgo)
                    .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToListAsync(); // Execute query

                // 2. Now in memory, format the data into your DTO
                statistics.UserGrowth = rawGrowthData
                    .Select(g => new UserGrowthDto
                    {
                        Month = $"{g.Year}-{g.Month:D2}", // This C# formatting is now fine
                        Count = g.Count
                    })
                    .ToList();

                return ApiResponse<UserStatisticsDto>.SuccessResponse(statistics, "Statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user statistics: {ex.Message}");
                return ApiResponse<UserStatisticsDto>.ErrorResponse("Failed to retrieve statistics");
            }
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email == email);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserId != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> CanDeleteUserAsync(int userId)
        {
            // Check if user has any related data
            // For now, just check if user exists
            // In future, check for classes, exams, materials, etc.
            var user = await _context.Users.FindAsync(userId);
            return user != null;
        }

        private string GenerateRandomPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%";
            var random = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[12];
                rng.GetBytes(buffer);

                for (int i = 0; i < 12; i++)
                {
                    random.Append(validChars[buffer[i] % validChars.Length]);
                }
            }

            // Ensure at least one of each type
            var password = random.ToString();
            password = "A" + password.Substring(1, 10) + "1@";

            return password;
        }
    }
}
