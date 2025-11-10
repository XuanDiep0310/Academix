using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models
{
    namespace Academix.WinApp.Models
    {
        public class LoginResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string Token { get; set; }  // Giữ tên này cho đơn giản
            public string RefreshToken { get; set; }
            public UserData User { get; set; }
        }

        public class UserData
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
        }
    }
}
