using Academix.Application.DTOs.Role;
using Academix.Application.Interfaces;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AcademixDbContext _context;

        public PermissionService(AcademixDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken ct = default)
        {
            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToListAsync(ct);

            return permissions;
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionCode, CancellationToken ct = default)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .AnyAsync(rp => rp.Permission.Code == permissionCode, ct);
        }

        public async Task<bool> HasAnyPermissionAsync(int userId, string[] permissionCodes, CancellationToken ct = default)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .AnyAsync(rp => permissionCodes.Contains(rp.Permission.Code), ct);
        }

        public async Task<bool> HasAllPermissionsAsync(int userId, string[] permissionCodes, CancellationToken ct = default)
        {
            var userPermissions = await GetUserPermissionsAsync(userId, ct);
            return permissionCodes.All(pc => userPermissions.Contains(pc));
        }

        public async Task<List<PermissionDto>> GetAllPermissionsAsync(CancellationToken ct = default)
        {
            return await _context.Permissions
                .Select(p => new PermissionDto
                {
                    PermissionId = p.PermissionId,
                    Name = p.Name,
                    Code = p.Code,
                    Description = p.Description
                })
                .ToListAsync(ct);
        }
    }
}
