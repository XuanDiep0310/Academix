using Academix.API.Extensions;
using Academix.Application.DTOs.Auth;
using Academix.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public PasswordController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        /// <summary>
        /// Change password (requires authentication)
        /// </summary>
        [HttpPost("change")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            if (request.NewPassword.Length < 8)
                return BadRequest(new { message = "Password must be at least 8 characters" });

            try
            {
                var userId = User.GetUserId();
                await _passwordService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

                return Ok(new { message = "Password changed successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Request password reset email
        /// </summary>
        [HttpPost("forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _passwordService.ForgotPasswordAsync(request.Email, ipAddress);

            // Always return success to prevent email enumeration
            return Ok(new
            {
                message = "If that email exists, a password reset link has been sent"
            });
        }

        /// <summary>
        /// Validate reset token
        /// </summary>
        [HttpGet("validate-reset-token")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
        {
            var isValid = await _passwordService.ValidateResetTokenAsync(token);

            if (!isValid)
                return BadRequest(new { message = "Invalid or expired token" });

            return Ok(new { message = "Token is valid" });
        }

        /// <summary>
        /// Reset password with token
        /// </summary>
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            if (request.NewPassword.Length < 8)
                return BadRequest(new { message = "Password must be at least 8 characters" });

            try
            {
                await _passwordService.ResetPasswordAsync(request.Token, request.NewPassword);
                return Ok(new { message = "Password reset successfully. You can now login with your new password." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
