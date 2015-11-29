using System;
#if USE_NET_CURRENT_PRINCIPAL
using ClaimsPrincipal = System.Security.Claims.ClaimsPrincipal;
#else
using ClaimsPrincipal = Odachi.Security.CurrentPrincipalFix;
#endif

namespace Odachi.Security
{
    /// <summary>
    /// Class representing a permission.
    /// </summary>
    public sealed class Permission
    {
        public const string PermissionClaim = "http://schemas.algorim.com/2015/09/odachi/claims/permission";

        public Permission(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; private set; }

        public bool IsGranted()
        {
            return ClaimsPrincipal.Current.HasPermission(this);
        }

        public void Demand()
        {
            ClaimsPrincipal.Current.DemandPermission(this);
        }
    }
}
