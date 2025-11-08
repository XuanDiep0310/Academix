using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Constants
{
    public static class TokenSettings
    {
        public const int AccessTokenExpirationMinutes = 60; // 1 hour
        public const int RefreshTokenExpirationDays = 7; // 7 days
        public const int PasswordResetTokenExpirationHours = 1; // 1 hour
    }
}
