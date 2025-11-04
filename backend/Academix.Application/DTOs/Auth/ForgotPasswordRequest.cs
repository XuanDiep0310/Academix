using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
