using System.Security;
using System.Security.Claims;

namespace Odachi.Security
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns whether principal has been granted a permission.
        /// </summary>
        /// <param name="principal">The principal</param>
        /// <param name="permission">The permission</param>
        /// <returns>True when permission is granted, false otherwise</returns>
        public static bool HasPermission(this ClaimsPrincipal principal, Permission permission)
        {
            return principal.HasClaim(Permission.PermissionClaim, permission.Name);
        }

        /// <summary>
        /// Throws exception when principal has not been granted a permission.
        /// </summary>
        /// <param name="principal">The principal</param>
        /// <param name="permission">The permission</param>
        public static void DemandPermission(this ClaimsPrincipal principal, Permission permission)
        {
            if (!HasPermission(principal, permission))
                throw new SecurityException("Principal '" + principal.Identity.Name + "' doesn't have permission '" + permission.Name + "'");
        }
    }
}
