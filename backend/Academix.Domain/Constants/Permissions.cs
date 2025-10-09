using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Constants
{
    public static class Permissions
    {
        // User Management
        public const string UserView = "user.view";
        public const string UserCreate = "user.create";
        public const string UserEdit = "user.edit";
        public const string UserDelete = "user.delete";
        public const string UserManageRoles = "user.manage_roles";

        // Role Management
        public const string RoleView = "role.view";
        public const string RoleCreate = "role.create";
        public const string RoleEdit = "role.edit";
        public const string RoleDelete = "role.delete";
        public const string RoleManagePermissions = "role.manage_permissions";

        // Organization Management
        public const string OrgView = "organization.view";
        public const string OrgEdit = "organization.edit";
        public const string OrgManageSettings = "organization.manage_settings";

        // Course Management (example for future)
        public const string CourseView = "course.view";
        public const string CourseCreate = "course.create";
        public const string CourseEdit = "course.edit";
        public const string CourseDelete = "course.delete";
        public const string CoursePublish = "course.publish";

        public static IEnumerable<(string Code, string Name, string Description)> GetAll()
        {
            return new[]
            {
                (UserView, "View Users", "Can view user list and details"),
                (UserCreate, "Create Users", "Can create new users"),
                (UserEdit, "Edit Users", "Can edit user information"),
                (UserDelete, "Delete Users", "Can delete or deactivate users"),
                (UserManageRoles, "Manage User Roles", "Can assign/remove roles from users"),

                (RoleView, "View Roles", "Can view role list and details"),
                (RoleCreate, "Create Roles", "Can create new roles"),
                (RoleEdit, "Edit Roles", "Can edit role information"),
                (RoleDelete, "Delete Roles", "Can delete roles"),
                (RoleManagePermissions, "Manage Role Permissions", "Can assign/remove permissions from roles"),

                (OrgView, "View Organization", "Can view organization details"),
                (OrgEdit, "Edit Organization", "Can edit organization information"),
                (OrgManageSettings, "Manage Organization Settings", "Can manage organization settings"),

                (CourseView, "View Courses", "Can view course list and details"),
                (CourseCreate, "Create Courses", "Can create new courses"),
                (CourseEdit, "Edit Courses", "Can edit course content"),
                (CourseDelete, "Delete Courses", "Can delete courses"),
                (CoursePublish, "Publish Courses", "Can publish/unpublish courses"),
            };
        }
    }
}
