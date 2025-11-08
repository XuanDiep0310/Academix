using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Constants
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";

        public static readonly string[] All = { Admin, Teacher, Student };

        public static bool IsValid(string role)
        {
            return All.Contains(role);
        }
    }
}
