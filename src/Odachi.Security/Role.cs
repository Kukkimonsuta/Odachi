using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Odachi.Security
{
    /// <summary>
    /// Class representing a role.
    /// </summary>
    public sealed class Role
    {
        public const string RoleClaim = ClaimTypes.Role;

        public Role(string name, Permission[] permissions)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (permissions == null)
                throw new ArgumentNullException(nameof(permissions));

            Name = name;
            Permissions = permissions
                .ToList()
                .AsReadOnly();
        }

        public string Name { get; private set; }
        public IReadOnlyCollection<Permission> Permissions { get; private set; }
    }
}
