using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(int userId, string email, List<string> roles, List<string> permissions);
        string GenerateRefreshToken();
        int? ValidateToken(string token);
    }

}
