using Academix.Application.DTOs.Auth;
using Academix.Application.Interfaces;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AcademixDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IRoleService _roleService;

        public AuthService(
            AcademixDbContext context,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IPermissionService permissionService,
            IRoleService roleService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _roleService = roleService;
        }

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var normalizedEmail = request.Email.ToUpperInvariant();

            var existingUser = await _context.Users
                .AnyAsync(u => u.NormalizedEmail == normalizedEmail &&
                              u.OrganizationId == request.OrganizationId, ct);

            if (existingUser)
                throw new InvalidOperationException("Email already registered");

            var (hash, salt) = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                NormalizedEmail = normalizedEmail,
                PasswordHash = hash,
                PasswordSalt = salt,
                DisplayName = request.DisplayName ?? request.Email.Split('@')[0],
                OrganizationId = request.OrganizationId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);

            return await GenerateLoginResponse(user, ct);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var normalizedEmail = request.Email.ToUpperInvariant();

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail &&
                                         u.IsActive && !u.IsDeleted, ct);

            if (user == null || user.PasswordHash == null || user.PasswordSalt == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid credentials");

            if (user.TwoFaenabled)
            {
                if (string.IsNullOrEmpty(request.TwoFACode))
                    throw new InvalidOperationException("2FA code required");
                // TODO: Implement 2FA verification
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            return await GenerateLoginResponse(user, ct);
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
        {
            // TODO: Implement refresh token validation and storage
            throw new NotImplementedException("Refresh token not implemented yet");
        }

        public async Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
        {
            var userId = _tokenService.ValidateToken(token);
            if (userId == null) return false;

            var user = await _context.Users
                .AnyAsync(u => u.UserId == userId && u.IsActive && !u.IsDeleted, ct);

            return user;
        }

        public async Task LogoutAsync(int userId, CancellationToken ct = default)
        {
            // TODO: Implement token blacklist or revocation
            await Task.CompletedTask;
        }

        public async Task<UserDto> GetCurrentUserAsync(int userId, CancellationToken ct = default)
        {
            var user = await _context.Users
                .Where(u => u.UserId == userId && u.IsActive && !u.IsDeleted)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    AvatarUrl = u.AvatarUrl,
                    OrganizationId = u.OrganizationId
                })
                .FirstOrDefaultAsync(ct);

            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Roles = await _roleService.GetUserRolesAsync(userId, ct);
            user.Permissions = await _permissionService.GetUserPermissionsAsync(userId, ct);

            return user;
        }

        private async Task<LoginResponse> GenerateLoginResponse(User user, CancellationToken ct)
        {
            var roles = await _roleService.GetUserRolesAsync(user.UserId, ct);
            var permissions = await _permissionService.GetUserPermissionsAsync(user.UserId, ct);

            var token = _tokenService.GenerateAccessToken(user.UserId, user.Email, roles, permissions);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    AvatarUrl = user.AvatarUrl,
                    OrganizationId = user.OrganizationId,
                    Roles = roles,
                    Permissions = permissions
                }
            };
        }
    }
}
