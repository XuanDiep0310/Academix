using Academix.API.Extensions;
using Academix.Application.DTOs.Auth;
using Academix.Application.Interfaces;
using Academix.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TwoFactorAuthController : ControllerBase
    {
        private readonly I2FAService _2faService;
        private readonly AcademixDbContext _context;

        public TwoFactorAuthController(I2FAService twoFactorAuthService, AcademixDbContext context)
        {
            _2faService = twoFactorAuthService;
            _context = context;
        }

        /// <summary>
        /// Step 1: Generate 2FA secret and QR code
        /// </summary>
        [HttpPost("setup")]
        public async Task<ActionResult<Enable2FAResponse>> Setup()
        {
            var userId = User.GetUserId();
            var email = User.GetEmail();

            var secret = _2faService.GenerateSecret();
            var qrCodeUrl = _2faService.GenerateQrCodeUrl(email, secret);

            return Ok(new Enable2FAResponse
            {
                Secret = secret,
                QrCodeUrl = qrCodeUrl,
                ManualEntryKey = secret // For manual entry in authenticator app
            });
        }

        /// <summary>
        /// Step 2: Verify code and enable 2FA
        /// </summary>
        [HttpPost("enable")]
        public async Task<IActionResult> Enable([FromQuery] string secret, [FromBody] Enable2FARequest request)
        {
            var userId = User.GetUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            // Get the secret from the request or from temporary storage
            // In production, you should store the secret temporarily during setup
            // For now, we'll get it from query parameter or assume it's in request
            //var secret = Request.Query["secret"].ToString();

            if (string.IsNullOrEmpty(secret))
                return BadRequest(new { message = "Secret is required" });

            // Verify the code
            if (!_2faService.ValidateCode(secret, request.Code))
                return BadRequest(new { message = "Invalid verification code" });

            // Enable 2FA
            await _2faService.Enable2FAAsync(userId, secret);

            return Ok(new { message = "2FA enabled successfully" });
        }

        /// <summary>
        /// Disable 2FA
        /// </summary>
        [HttpPost("disable")]
        public async Task<IActionResult> Disable([FromBody] Verify2FARequest request)
        {
            var userId = User.GetUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null || !user.TwoFaenabled)
                return BadRequest(new { message = "2FA is not enabled" });

            // Verify current code before disabling
            if (!_2faService.ValidateCode(user.TwoFasecret!, request.Code))
                return BadRequest(new { message = "Invalid verification code" });

            await _2faService.Disable2FAAsync(userId);

            return Ok(new { message = "2FA disabled successfully" });
        }

        /// <summary>
        /// Check 2FA status
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var userId = User.GetUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                enabled = user.TwoFaenabled,
                hasSecret = !string.IsNullOrEmpty(user.TwoFasecret)
            });
        }

        /// <summary>
        /// Generate backup codes (optional feature)
        /// </summary>
        [HttpPost("backup-codes")]
        public IActionResult GenerateBackupCodes()
        {
            // Generate 10 backup codes
            var backupCodes = new List<string>();
            var random = new Random();

            for (int i = 0; i < 10; i++)
            {
                var code = random.Next(100000, 999999).ToString();
                backupCodes.Add(code);
            }

            // TODO: Store hashed backup codes in database
            // For now, just return them

            return Ok(new
            {
                message = "Save these backup codes in a safe place",
                codes = backupCodes
            });
        }
    }
}
