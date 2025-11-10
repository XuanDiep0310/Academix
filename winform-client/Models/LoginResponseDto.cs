using System;

namespace Academix.WinApp.Models
{
    /// <summary>
    /// DTO từ API response
    /// </summary>
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDto User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}