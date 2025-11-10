using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Users
{
    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public List<UserGrowthDto> UserGrowth { get; set; } = new();
    }

    public class UserGrowthDto
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
