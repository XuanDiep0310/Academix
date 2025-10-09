using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Constants
{
    public static class DefaultRoles
    {
        public const string SystemAdmin = "System Admin";
        public const string OrgAdmin = "Organization Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";

        public static Dictionary<string, string[]> GetRolePermissions()
        {
            return new Dictionary<string, string[]>
            {
                [SystemAdmin] = new[]
                {
                    Permissions.UserView, Permissions.UserCreate, Permissions.UserEdit,
                    Permissions.UserDelete, Permissions.UserManageRoles,
                    Permissions.RoleView, Permissions.RoleCreate, Permissions.RoleEdit,
                    Permissions.RoleDelete, Permissions.RoleManagePermissions,
                    Permissions.OrgView, Permissions.OrgEdit, Permissions.OrgManageSettings,
                    Permissions.CourseView, Permissions.CourseCreate, Permissions.CourseEdit,
                    Permissions.CourseDelete, Permissions.CoursePublish,
                },
                [OrgAdmin] = new[]
                {
                    Permissions.UserView, Permissions.UserCreate, Permissions.UserEdit,
                    Permissions.UserManageRoles,
                    Permissions.RoleView, Permissions.RoleCreate, Permissions.RoleEdit,
                    Permissions.OrgView, Permissions.OrgEdit,
                    Permissions.CourseView, Permissions.CourseCreate, Permissions.CourseEdit,
                    Permissions.CourseDelete, Permissions.CoursePublish,
                },
                [Teacher] = new[]
                {
                    Permissions.UserView,
                    Permissions.CourseView, Permissions.CourseCreate, Permissions.CourseEdit,
                },
                [Student] = new[]
                {
                    Permissions.CourseView,
                }
            };
        }
    }
}
