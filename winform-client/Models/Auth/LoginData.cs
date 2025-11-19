using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Auth
{
    public class LoginData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserData User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
