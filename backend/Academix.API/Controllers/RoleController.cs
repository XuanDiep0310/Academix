using Academix.API.Attributes;
using Academix.API.Extensions;
using Academix.Application.DTOs.Role;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [RequirePermission(Permissions.RoleView)]
        public async Task<ActionResult<List<RoleDto>>> GetRoles([FromQuery] int? organizationId)
        {
            var roles = await _roleService.GetRolesByOrganizationAsync(organizationId);
            return Ok(roles);
        }

        [HttpGet("{id}")]
        [RequirePermission(Permissions.RoleView)]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();
            return Ok(role);
        }

        [HttpPost]
        [RequirePermission(Permissions.RoleCreate)]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(request);
                return CreatedAtAction(nameof(GetRole), new { id = role.RoleId }, role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [RequirePermission(Permissions.RoleEdit)]
        public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] CreateRoleRequest request)
        {
            try
            {
                var role = await _roleService.UpdateRoleAsync(id, request);
                return Ok(role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [RequirePermission(Permissions.RoleDelete)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost("assign")]
        [RequirePermission(Permissions.UserManageRoles)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var userId = User.GetUserId();
            var result = await _roleService.AssignRoleToUserAsync(request.UserId, request.RoleId, userId);

            if (!result)
                return BadRequest(new { message = "Role already assigned to user" });

            return Ok(new { message = "Role assigned successfully" });
        }

        [HttpPost("remove")]
        [RequirePermission(Permissions.UserManageRoles)]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleRequest request)
        {
            var result = await _roleService.RemoveRoleFromUserAsync(request.UserId, request.RoleId);

            if (!result)
                return NotFound(new { message = "Role assignment not found" });

            return Ok(new { message = "Role removed successfully" });
        }

        [HttpGet("user/{userId}")]
        [RequirePermission(Permissions.UserView)]
        public async Task<ActionResult<List<string>>> GetUserRoles(int userId)
        {
            var roles = await _roleService.GetUserRolesAsync(userId);
            return Ok(roles);
        }
    }
}
