using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface I2FAService
    {
        string GenerateSecret();
        string GenerateQrCodeUrl(string email, string secret, string issuer = "Academix");
        bool ValidateCode(string secret, string code);
        Task<bool> Enable2FAAsync(int userId, string secret, CancellationToken ct = default);
        Task<bool> Disable2FAAsync(int userId, CancellationToken ct = default);
    }
}
