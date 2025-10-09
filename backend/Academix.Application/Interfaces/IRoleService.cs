using Academix.Application.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleDto> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default);
        Task<RoleDto> UpdateRoleAsync(int roleId, CreateRoleRequest request, CancellationToken ct = default);
        Task<bool> DeleteRoleAsync(int roleId, CancellationToken ct = default);
        Task<RoleDto?> GetRoleByIdAsync(int roleId, CancellationToken ct = default);
        Task<List<RoleDto>> GetRolesByOrganizationAsync(int? organizationId, CancellationToken ct = default);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId, int assignedBy, CancellationToken ct = default);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken ct = default);
        Task<List<string>> GetUserRolesAsync(int userId, CancellationToken ct = default);
    }
}
