using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IPasswordService
    {
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default);
        Task<bool> ForgotPasswordAsync(string email, string? ipAddress = null, CancellationToken ct = default);
        Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken ct = default);
        Task<bool> ValidateResetTokenAsync(string token, CancellationToken ct = default);
    }
}
