using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Auth
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}
