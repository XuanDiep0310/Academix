using Academix.Application.DTOs.Auth;
using Academix.Application.DTOs.Common;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academix.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AcademixDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            AcademixDbContext context,
            IJwtService jwtService,
            IEmailService emailService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !user.IsActive)
                {
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email or password");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email or password");
                }

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user.UserId, user.Email, user.Role);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Save refresh token
                var refreshTokenEntity = new RefreshToken
                {
                    UserId = user.UserId,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(TokenSettings.RefreshTokenExpirationDays),
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var response = new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(TokenSettings.AccessTokenExpirationMinutes),
                    User = new UserDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role,
                        IsActive = user.IsActive
                    }
                };

                _logger.LogInformation($"User {user.Email} logged in successfully");
                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return ApiResponse<LoginResponseDto>.ErrorResponse("An error occurred during login");
            }
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                // Validate role
                if (!UserRoles.IsValid(request.Role) || request.Role == UserRoles.Admin)
                {
                    return ApiResponse<UserDto>.ErrorResponse("Invalid role");
                }

                // Check if email exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (existingUser != null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("Email already exists");
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
                    _logger.LogWarning($"Failed to send welcome email: {ex.Message}");
                }

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive
                };

                _logger.LogInformation($"User {user.Email} registered successfully");
                return ApiResponse<UserDto>.SuccessResponse(userDto, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration error: {ex.Message}");
                return ApiResponse<UserDto>.ErrorResponse("An error occurred during registration");
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            try
            {
                var refreshToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked);

                if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid or expired refresh token");
                }

                var user = refreshToken.User;

                if (!user.IsActive)
                {
                    return ApiResponse<LoginResponseDto>.ErrorResponse("User account is inactive");
                }

                // Generate new tokens
                var newAccessToken = _jwtService.GenerateAccessToken(user.UserId, user.Email, user.Role);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Revoke old refresh token
                refreshToken.IsRevoked = true;

                // Create new refresh token
                var newRefreshTokenEntity = new RefreshToken
                {
                    UserId = user.UserId,
                    Token = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(TokenSettings.RefreshTokenExpirationDays),
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(newRefreshTokenEntity);
                await _context.SaveChangesAsync();

                var response = new LoginResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(TokenSettings.AccessTokenExpirationMinutes),
                    User = new UserDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role,
                        IsActive = user.IsActive
                    }
                };

                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Refresh token error: {ex.Message}");
                return ApiResponse<LoginResponseDto>.ErrorResponse("An error occurred while refreshing token");
            }
        }

        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (token != null)
                {
                    token.IsRevoked = true;
                    await _context.SaveChangesAsync();
                }

                return ApiResponse<string>.SuccessResponse("Logged out successfully", "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Logout error: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("An error occurred during logout");
            }
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return ApiResponse<string>.ErrorResponse("User not found");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                {
                    return ApiResponse<string>.ErrorResponse("Current password is incorrect");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {user.Email} changed password successfully");
                return ApiResponse<string>.SuccessResponse("Password changed successfully", "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Change password error: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("An error occurred while changing password");
            }
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    // Return success even if user doesn't exist (security best practice)
                    return ApiResponse<string>.SuccessResponse(
                        "If the email exists, a password reset link has been sent",
                        "Password reset email sent");
                }

                // Generate reset token
                var resetToken = Guid.NewGuid().ToString("N");
                var passwordResetToken = new PasswordResetToken
                {
                    UserId = user.UserId,
                    Token = resetToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(TokenSettings.PasswordResetTokenExpirationHours),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PasswordResetTokens.Add(passwordResetToken);
                await _context.SaveChangesAsync();

                // Send reset email
                await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken, user.FullName);

                _logger.LogInformation($"Password reset email sent to {user.Email}");
                return ApiResponse<string>.SuccessResponse(
                    "Password reset link has been sent to your email",
                    "Password reset email sent");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Forgot password error: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("An error occurred while processing your request");
            }
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            try
            {
                var resetToken = await _context.PasswordResetTokens
                    .Include(prt => prt.User)
                    .FirstOrDefaultAsync(prt => prt.Token == request.Token && !prt.IsUsed);

                if (resetToken == null || resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    return ApiResponse<string>.ErrorResponse("Invalid or expired reset token");
                }

                var user = resetToken.User;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                resetToken.IsUsed = true;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {user.Email} reset password successfully");
                return ApiResponse<string>.SuccessResponse("Password reset successfully", "Password reset successful");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Reset password error: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("An error occurred while resetting password");
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }
    }
}
