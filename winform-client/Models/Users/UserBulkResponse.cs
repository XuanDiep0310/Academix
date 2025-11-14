using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Users
{
    public class UserBulkResponse
    {
        public List<UserSuccessInfo> SuccessfulUsers { get; set; } = new();
        public List<UserFailedInfo> FailedUsers { get; set; } = new();
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class UserSuccessInfo
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string TemporaryPassword { get; set; }
    }

    public class UserFailedInfo
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Reason { get; set; }
    }
}
