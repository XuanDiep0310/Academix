using Academix.Application.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken ct = default);
        Task<bool> HasPermissionAsync(int userId, string permissionCode, CancellationToken ct = default);
        Task<bool> HasAnyPermissionAsync(int userId, string[] permissionCodes, CancellationToken ct = default);
        Task<bool> HasAllPermissionsAsync(int userId, string[] permissionCodes, CancellationToken ct = default);
        Task<List<PermissionDto>> GetAllPermissionsAsync(CancellationToken ct = default);
    }
}
