using System;

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
    }
}
