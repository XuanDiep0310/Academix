using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Role
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? OrganizationId { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}
