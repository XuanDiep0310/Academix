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
    public class EmailConfirmationService : IEmailConfirmationService
    {
        private readonly AcademixDbContext _context;
        private readonly IEmailService _emailService;
        private readonly string _baseUrl;

        public EmailConfirmationService(
            AcademixDbContext context,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _baseUrl = configuration["AppSettings:BaseUrl"] ?? "https://localhost:5001";
        }

        public async Task<string> GenerateConfirmationTokenAsync(int userId, CancellationToken ct = default)
        {
            // Generate secure token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            var confirmationToken = new EmailConfirmationToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24), // 24 hours
                CreatedAt = DateTime.UtcNow
            };

            _context.EmailConfirmationTokens.Add(confirmationToken);
            await _context.SaveChangesAsync(ct);

            // Get user email
            var user = await _context.Users.FindAsync(new object[] { userId }, ct);
            if (user != null)
            {
                var confirmationLink = $"{_baseUrl}/api/auth/confirm-email?token={token}";
                await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
            }

            return token;
        }

        public async Task<bool> ConfirmEmailAsync(string token, CancellationToken ct = default)
        {
            var confirmationToken = await _context.EmailConfirmationTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token, ct);

            if (confirmationToken == null || !confirmationToken.IsValid)
                return false;

            var user = confirmationToken.User;
            user.IsEmailConfirmed = true;

            confirmationToken.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            // Send welcome email
            await _emailService.SendWelcomeEmailAsync(user.Email, user.DisplayName ?? "User");

            Console.WriteLine($"[EMAIL] Email confirmed for user: {user.Email}");
            return true;
        }

        public async Task ResendConfirmationEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user == null)
                throw new InvalidOperationException("User not found");

            if (user.IsEmailConfirmed)
                throw new InvalidOperationException("Email already confirmed");

            // Invalidate old tokens
            var oldTokens = await _context.EmailConfirmationTokens
                .Where(t => t.UserId == user.UserId && t.UsedAt == null)
                .ToListAsync(ct);

            foreach (var oldToken in oldTokens)
            {
                oldToken.UsedAt = DateTime.UtcNow;
            }

            // Generate new token
            await GenerateConfirmationTokenAsync(user.UserId, ct);
        }
    }
}
