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
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [RequirePermission(Permissions.RoleView)]
        public async Task<ActionResult<List<PermissionDto>>> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpGet("user/{userId}")]
        [RequirePermission(Permissions.UserView)]
        public async Task<ActionResult<List<string>>> GetUserPermissions(int userId)
        {
            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            return Ok(permissions);
        }

        [HttpGet("me")]
        public async Task<ActionResult<List<string>>> GetMyPermissions()
        {
            var userId = User.GetUserId();
            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            return Ok(permissions);
        }

        [HttpPost("check")]
        public async Task<ActionResult<bool>> CheckPermission([FromBody] CheckPermissionRequest request)
        {
            var userId = User.GetUserId();
            var hasPermission = await _permissionService.HasPermissionAsync(userId, request.PermissionCode);
            return Ok(new { hasPermission });
        }
    }

    public class CheckPermissionRequest
    {
        public string PermissionCode { get; set; } = string.Empty;
    }
}
