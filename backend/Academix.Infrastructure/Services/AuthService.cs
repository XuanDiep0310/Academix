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
        private readonly I2FAService _2faService;


        public AuthService(
            AcademixDbContext context,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IPermissionService permissionService,
            IRoleService roleService,
            I2FAService twoFactorAuthService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _roleService = roleService;
            _2faService = twoFactorAuthService;
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
                IsDeleted = false,
                IsEmailConfirmed = false // Cần confirm email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);

            // TODO: Send confirmation email
            Console.WriteLine($"[AUTH] User registered: {user.Email} (UserId: {user.UserId})");

            return await GenerateLoginResponse(user, null, ct);
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

            // Check 2FA
            if (user.TwoFaenabled)
            {
                if (string.IsNullOrEmpty(request.TwoFACode))
                    throw new InvalidOperationException("2FA code required");

                if (!_2faService.ValidateCode(user.TwoFasecret!, request.TwoFACode))
                    throw new UnauthorizedAccessException("Invalid 2FA code");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            return await GenerateLoginResponse(user, request.IpAddress, ct);
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
        {
            var user = await GetUserByRefreshTokenAsync(request.RefreshToken, ct);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, ct);

            if (refreshToken == null || refreshToken.IsActive == 0)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            // Revoke old token and create new one
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = request.IpAddress;
            refreshToken.ReasonRevoked = "Replaced by new token";

            var newLoginResponse = await GenerateLoginResponse(user, request.IpAddress, ct);

            refreshToken.ReplacedByToken = newLoginResponse.RefreshToken;
            await _context.SaveChangesAsync(ct);

            Console.WriteLine($"[AUTH] Token refreshed for user: {user.Email}");

            return newLoginResponse;
        }

        public async Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
        {
            var userId = _tokenService.ValidateToken(token);
            if (userId == null) return false;

            // Check if token is blacklisted
            var isBlacklisted = await _context.TokenBlacklists
                .AnyAsync(tb => tb.Token == token && tb.ExpiresAt > DateTime.UtcNow, ct);

            if (isBlacklisted) return false;

            var user = await _context.Users
                .AnyAsync(u => u.UserId == userId && u.IsActive && !u.IsDeleted, ct);

            return user;
        }

        public async Task LogoutAsync(int userId, CancellationToken ct = default)
        {
            // Revoke all active refresh tokens
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(ct);

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.ReasonRevoked = "User logout";
            }

            await _context.SaveChangesAsync(ct);
            Console.WriteLine($"[AUTH] User logged out: {userId} ({activeTokens.Count} tokens revoked)");
        }

        public async Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken ct = default)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken, ct);

            if (token == null || token.IsActive == 0)
                throw new InvalidOperationException("Invalid token");

            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = "Revoked without replacement";

            await _context.SaveChangesAsync(ct);
        }

        public async Task BlacklistTokenAsync(string accessToken, int userId, DateTime expiresAt, string? reason = null, CancellationToken ct = default)
        {
            var blacklist = new TokenBlacklist
            {
                Token = accessToken,
                UserId = userId,
                ExpiresAt = expiresAt,
                RevokedAt = DateTime.UtcNow,
                Reason = reason
            };

            _context.TokenBlacklists.Add(blacklist);
            await _context.SaveChangesAsync(ct);
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

        private async Task<LoginResponse> GenerateLoginResponse(User user, string? ipAddress, CancellationToken ct)
        {
            var roles = await _roleService.GetUserRolesAsync(user.UserId, ct);
            var permissions = await _permissionService.GetUserPermissionsAsync(user.UserId, ct);

            var accessToken = _tokenService.GenerateAccessToken(user.UserId, user.Email, roles, permissions);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync(ct);

            return new LoginResponse
            {
                Token = accessToken,
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

        private async Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken ct)
        {
            var token = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken, ct);

            return token?.User;
        }

        private bool Verify2FACode(string? secret, string code)
        {
            if (string.IsNullOrEmpty(secret)) return false;

            // TODO: Implement TOTP verification
            // Using library like OtpNet or Google.Authenticator
            return true; // Placeholder
        }
    }
}
