using Academix.Application.Interfaces;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly AcademixDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly string _baseUrl;

        public PasswordService(
            AcademixDbContext context,
            IPasswordHasher passwordHasher,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _baseUrl = configuration["AppSettings:BaseUrl"] ?? "https://localhost:7102";
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, ct);

            if (user == null || user.PasswordHash == null || user.PasswordSalt == null)
                throw new InvalidOperationException("User not found");

            // Verify current password
            if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Current password is incorrect");

            // Validate new password
            if (currentPassword == newPassword)
                throw new InvalidOperationException("New password must be different from current password");

            // Hash new password
            var (hash, salt) = _passwordHasher.HashPassword(newPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            Console.WriteLine($"[PASSWORD] Password changed for user: {user.Email}");
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email, string? ipAddress = null, CancellationToken ct = default)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive && !u.IsDeleted, ct);

            // Always return true to prevent email enumeration
            if (user == null)
            {
                Console.WriteLine($"[PASSWORD] Password reset requested for non-existent email: {email}");
                return true;
            }

            // Generate secure token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            var resetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1), // 1 hour expiration
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync(ct);

            // Send reset email
            var resetLink = $"{_baseUrl}/reset-password?token={token}";
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

            Console.WriteLine($"[PASSWORD] Password reset email sent to: {user.Email}");
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken ct = default)
        {
            var resetToken = await _context.PasswordResetTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, ct);

            if (resetToken == null || !resetToken.IsValid)
                throw new InvalidOperationException("Invalid or expired reset token");

            var user = resetToken.User;

            // Hash new password
            var (hash, salt) = _passwordHasher.HashPassword(newPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.UpdatedAt = DateTime.UtcNow;

            // Mark token as used
            resetToken.UsedAt = DateTime.UtcNow;

            // Revoke all refresh tokens for security
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == user.UserId && rt.RevokedAt == null)
                .ToListAsync(ct);

            foreach (var rt in refreshTokens)
            {
                rt.RevokedAt = DateTime.UtcNow;
                rt.ReasonRevoked = "Password reset";
            }

            await _context.SaveChangesAsync(ct);

            Console.WriteLine($"[PASSWORD] Password reset successful for user: {user.Email}");
            return true;
        }

        public async Task<bool> ValidateResetTokenAsync(string token, CancellationToken ct = default)
        {
            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(rt => rt.Token == token, ct);

            return resetToken != null && resetToken.IsValid;
        }
    }
}
