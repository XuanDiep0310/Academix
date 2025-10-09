using Academix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Academix.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _permissions;
        private readonly bool _requireAll;

        public RequirePermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
            _requireAll = false;
        }

        public RequirePermissionAttribute(bool requireAll, params string[] permissions)
        {
            _permissions = permissions;
            _requireAll = requireAll;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionService = context.HttpContext.RequestServices
                .GetRequiredService<IPermissionService>();

            bool hasPermission;

            if (_requireAll)
            {
                hasPermission = await permissionService.HasAllPermissionsAsync(userId, _permissions);
            }
            else
            {
                hasPermission = await permissionService.HasAnyPermissionAsync(userId, _permissions);
            }

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
