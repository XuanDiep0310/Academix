using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IEmailConfirmationService
    {
        Task<string> GenerateConfirmationTokenAsync(int userId, CancellationToken ct = default);
        Task<bool> ConfirmEmailAsync(string token, CancellationToken ct = default);
        Task ResendConfirmationEmailAsync(string email, CancellationToken ct = default);
    }
}
