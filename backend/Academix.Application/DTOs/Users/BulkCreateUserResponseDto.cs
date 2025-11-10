using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Users
{
    public class BulkCreateUserResponseDto
    {
        public List<UserCreatedDto> SuccessfulUsers { get; set; } = new();
        public List<UserCreationFailedDto> FailedUsers { get; set; } = new();
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class UserCreatedDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string TemporaryPassword { get; set; } = string.Empty;
    }

    public class UserCreationFailedDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
