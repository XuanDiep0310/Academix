using Academix.Application.Interfaces;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class TwoFactorAuthService : I2FAService
    {
        private readonly AcademixDbContext _context;

        public TwoFactorAuthService(AcademixDbContext context)
        {
            _context = context;
        }

        public string GenerateSecret()
        {
            // Generate a random secret key (Base32 encoded)
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateQrCodeUrl(string email, string secret, string issuer = "Academix")
        {
            // Format: otpauth://totp/Issuer:Email?secret=SECRET&issuer=Issuer
            var encodedEmail = Uri.EscapeDataString(email);
            var encodedIssuer = Uri.EscapeDataString(issuer);

            return $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={secret}&issuer={encodedIssuer}";
        }

        public bool ValidateCode(string secret, string code)
        {
            try
            {
                var secretBytes = Base32Encoding.ToBytes(secret);
                var totp = new Totp(secretBytes);

                // Verify with a time window of ±1 step (30 seconds each)
                long timeStepMatched;
                var isValid = totp.VerifyTotp(code, out timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                return isValid;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Enable2FAAsync(int userId, string secret, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, ct);

            if (user == null)
                return false;

            user.TwoFaenabled = true;
            user.TwoFasecret = secret;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> Disable2FAAsync(int userId, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, ct);

            if (user == null)
                return false;

            user.TwoFaenabled = false;
            user.TwoFasecret = null;

            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
