using System.Security;
using System.Security.Claims;
using System.Security.Principal;

namespace Odachi.Security
{
    public static class PrincipalExtensions
    {
        /// <summary>
        /// Returns whether principal has been granted a permission.
        /// </summary>
        /// <param name="principal">The principal</param>
        /// <param name="permission">The permission</param>
        /// <returns>True when permission is granted, false otherwise</returns>
        public static bool HasPermission(this ClaimsPrincipal principal, Permission permission)
        {
			foreach (var claim in principal.Claims)
			{
				if (claim.Type != Permission.PermissionClaim)
					continue;

				if (permission.Matches(claim.Value))
					return true;
			}

			return false;
        }
		/// <summary>
		/// Returns whether principal has been granted a permission.
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="arg0">The first permission argument</param>
		/// <returns>True when permission is granted, false otherwise</returns>
		public static bool HasPermission(this ClaimsPrincipal principal, Permission permission, object arg0)
		{
			foreach (var claim in principal.Claims)
			{
				if (claim.Type != Permission.PermissionClaim)
					continue;

				if (permission.Matches(claim.Value, arg0))
					return true;
			}

			return false;
		}
		/// <summary>
		/// Returns whether principal has been granted a permission.
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="arg0">The first permission argument</param>
		/// <param name="arg1">The second permission argument</param>
		/// <returns>True when permission is granted, false otherwise</returns>
		public static bool HasPermission(this ClaimsPrincipal principal, Permission permission, object arg0, object arg1)
		{
			foreach (var claim in principal.Claims)
			{
				if (claim.Type != Permission.PermissionClaim)
					continue;

				if (permission.Matches(claim.Value, arg0, arg1))
					return true;
			}

			return false;
		}
		/// <summary>
		/// Returns whether principal has been granted a permission.
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="args">Permission arguments</param>
		/// <returns>True when permission is granted, false otherwise</returns>
		public static bool HasPermission(this ClaimsPrincipal principal, Permission permission, params object[] args)
		{
			foreach (var claim in principal.Claims)
			{
				if (claim.Type != Permission.PermissionClaim)
					continue;

				if (permission.Matches(claim.Value, args))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Throws exception when principal has not been granted a permission.
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		public static void DemandPermission(this ClaimsPrincipal principal, Permission permission)
        {
            if (!HasPermission(principal, permission))
                throw new SecurityException("Principal '" + principal.Identity.Name + "' doesn't have permission '" + permission.Template + "'");
        }
		/// <summary>
		/// Throws exception when principal has not been granted a permission.
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="arg0">The first permission argument</param>
		public static void DemandPermission(this ClaimsPrincipal principal, Permission permission, object arg0)
		{
			if (!HasPermission(principal, permission, arg0))
				throw new SecurityException("Principal '" + principal.Identity.Name + "' doesn't have permission '" + permission.Template + "'+1");
		}
		/// <summary>
		/// Throws exception when principal has not been granted a permission.
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="arg0">The first permission argument</param>
		/// <param name="arg1">The second permission argument</param>
		public static void DemandPermission(this ClaimsPrincipal principal, Permission permission, object arg0, object arg1)
		{
			if (!HasPermission(principal, permission, arg0, arg1))
				throw new SecurityException("Principal '" + principal.Identity.Name + "' doesn't have permission '" + permission.Template + "'+2");
		}
		/// <summary>
		/// Throws exception when principal has not been granted a permission.
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="args">Permission arguments</param>
		public static void DemandPermission(this ClaimsPrincipal principal, Permission permission, params object[] args)
		{
			if (!HasPermission(principal, permission, args))
				throw new SecurityException("Principal '" + principal.Identity.Name + "' doesn't have permission '" + permission.Template + "'+" + args.Length);
		}

		/// <summary>
		/// Returns whether principal has been granted a permission (assumes principal is ClaimsPrincipal).
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <returns>True when permission is granted, false otherwise</returns>
		public static bool HasPermission(this IPrincipal principal, Permission permission)
		{
			return ((ClaimsPrincipal)principal).HasPermission(permission);
		}
		/// <summary>
		/// Returns whether principal has been granted a permission (assumes principal is ClaimsPrincipal).
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="arg0">The first permission argument</param>
		/// <returns>True when permission is granted, false otherwise</returns>
		public static bool HasPermission(this IPrincipal principal, Permission permission, object arg0)
		{
			return ((ClaimsPrincipal)principal).HasPermission(permission, arg0);
		}
		/// <summary>
		/// Returns whether principal has been granted a permission (assumes principal is ClaimsPrincipal).
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="arg0">The first permission argument</param>
		/// <param name="arg1">The second permission argument</param>
		/// <returns>True when permission is granted, false otherwise</returns>
		public static bool HasPermission(this IPrincipal principal, Permission permission, object arg0, object arg1)
		{
			return ((ClaimsPrincipal)principal).HasPermission(permission, arg0, arg1);
		}
		/// <summary>
		/// Returns whether principal has been granted a permission (assumes principal is ClaimsPrincipal).
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="args">Permission arguments</param>
		/// <returns>True when permission is granted, false otherwise</returns>
		public static bool HasPermission(this IPrincipal principal, Permission permission, params object[] args)
		{
			return ((ClaimsPrincipal)principal).HasPermission(permission, args);
		}

		/// <summary>
		/// Throws exception when principal has not been granted a permission (assumes principal is ClaimsPrincipal).
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		public static void DemandPermission(this IPrincipal principal, Permission permission)
		{
			((ClaimsPrincipal)principal).DemandPermission(permission);
		}
		/// <summary>
		/// Throws exception when principal has not been granted a permission (assumes principal is ClaimsPrincipal).
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="arg0">The first permission argument</param>
		public static void DemandPermission(this IPrincipal principal, Permission permission, object arg0)
		{
			((ClaimsPrincipal)principal).DemandPermission(permission, arg0);
		}
		/// <summary>
		/// Throws exception when principal has not been granted a permission (assumes principal is ClaimsPrincipal).
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="arg0">The first permission argument</param>
		/// <param name="arg1">The second permission argument</param>
		public static void DemandPermission(this IPrincipal principal, Permission permission, object arg0, object arg1)
		{
			((ClaimsPrincipal)principal).DemandPermission(permission, arg0, arg1);
		}
		/// <summary>
		/// Throws exception when principal has not been granted a permission (assumes principal is ClaimsPrincipal).
		/// </summary>
		/// <param name="principal">The principal</param>
		/// <param name="permission">The permission</param>
		/// <param name="args">Permission arguments</param>
		public static void DemandPermission(this IPrincipal principal, Permission permission, params object[] args)
		{
			((ClaimsPrincipal)principal).DemandPermission(permission, args);
		}
	}
}
