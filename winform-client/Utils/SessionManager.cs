using Academix.WinApp.Models;
using Academix.WinApp.Models.Academix.WinApp.Models;
using System.IO.Packaging;

namespace Academix.WinApp.Utils
{
    public static class SessionManager
    {
        public static string Token { get; set; }
        public static string RefreshToken { get; set; }
        public static UserData CurrentUser { get; set; }

        public static bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        public static void ClearSession()
        {
            Token = null;
            RefreshToken = null;
            CurrentUser = null;
        }
    }
}
