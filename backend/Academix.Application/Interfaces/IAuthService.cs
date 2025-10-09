using Academix.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
        Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default);
        Task LogoutAsync(int userId, CancellationToken ct = default);
        Task<UserDto> GetCurrentUserAsync(int userId, CancellationToken ct = default);
    }
}
