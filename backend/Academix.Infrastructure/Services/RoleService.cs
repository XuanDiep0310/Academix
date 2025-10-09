using Microsoft.EntityFrameworkCore;
using Academix.Application.DTOs.Role;
using Academix.Application.Interfaces;
using Academix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly AcademixDbContext _context;

        public RoleService(AcademixDbContext context)
        {
            _context = context;
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default)
        {
            var existingRole = await _context.Roles
                .AnyAsync(r => r.Name == request.Name &&
                              r.OrganizationId == request.OrganizationId, ct);

            if (existingRole)
                throw new InvalidOperationException("Role with this name already exists");

            var role = new Role
            {
                Name = request.Name,
                Description = request.Description,
                OrganizationId = request.OrganizationId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync(ct);

            if (request.PermissionIds.Any())
            {
                var rolePermissions = request.PermissionIds.Select(permId => new RolePermission
                {
                    RoleId = role.RoleId,
                    PermissionId = permId
                });

                _context.RolePermissions.AddRange(rolePermissions);
                await _context.SaveChangesAsync(ct);
            }

            return await GetRoleByIdAsync(role.RoleId, ct)
                ?? throw new InvalidOperationException("Failed to create role");
        }

        public async Task<RoleDto> UpdateRoleAsync(int roleId, CreateRoleRequest request, CancellationToken ct = default)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.RoleId == roleId, ct);

            if (role == null)
                throw new InvalidOperationException("Role not found");

            role.Name = request.Name;
            role.Description = request.Description;

            _context.RolePermissions.RemoveRange(role.RolePermissions);

            var newPermissions = request.PermissionIds.Select(permId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = permId
            });

            _context.RolePermissions.AddRange(newPermissions);
            await _context.SaveChangesAsync(ct);

            return await GetRoleByIdAsync(roleId, ct)
                ?? throw new InvalidOperationException("Failed to update role");
        }

        public async Task<bool> DeleteRoleAsync(int roleId, CancellationToken ct = default)
        {
            var role = await _context.Roles.FindAsync(new object[] { roleId }, ct);
            if (role == null) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int roleId, CancellationToken ct = default)
        {
            return await _context.Roles
                .Where(r => r.RoleId == roleId)
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    Name = r.Name,
                    Description = r.Description,
                    OrganizationId = r.OrganizationId,
                    Permissions = r.RolePermissions.Select(rp => new PermissionDto
                    {
                        PermissionId = rp.Permission.PermissionId,
                        Name = rp.Permission.Name,
                        Code = rp.Permission.Code,
                        Description = rp.Permission.Description
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<RoleDto>> GetRolesByOrganizationAsync(int? organizationId, CancellationToken ct = default)
        {
            return await _context.Roles
                .Where(r => r.OrganizationId == organizationId)
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    Name = r.Name,
                    Description = r.Description,
                    OrganizationId = r.OrganizationId,
                    Permissions = r.RolePermissions.Select(rp => new PermissionDto
                    {
                        PermissionId = rp.Permission.PermissionId,
                        Name = rp.Permission.Name,
                        Code = rp.Permission.Code,
                        Description = rp.Permission.Description
                    }).ToList()
                })
                .ToListAsync(ct);
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId, int assignedBy, CancellationToken ct = default)
        {
            var exists = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);

            if (exists) return false;

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken ct = default)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);

            if (userRole == null) return false;

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<List<string>> GetUserRolesAsync(int userId, CancellationToken ct = default)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync(ct);
        }
    }
}
