using System.Security.Claims;

namespace Academix.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("User ID not found in token");
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("Email not found in token");
        }

        public static List<string> GetRoles(this ClaimsPrincipal principal)
        {
            return principal.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }

        public static List<string> GetPermissions(this ClaimsPrincipal principal)
        {
            return principal.FindAll("permission")
                .Select(c => c.Value)
                .ToList();
        }

        public static bool HasPermission(this ClaimsPrincipal principal, string permission)
        {
            return principal.FindAll("permission")
                .Any(c => c.Value == permission);
        }
    }
}
