#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Odachi.Security
{
    /// <summary>
    /// Class representing a role.
    /// </summary>
    public sealed class Role : ClaimTemplate
	{
        public const string RoleClaim = ClaimTypes.Role;

        public Role(string template, Permission[]? permissions = null)
			: base(template)
        {
			Name = template ?? throw new ArgumentNullException(nameof(template));
            Permissions = permissions == null ? Array.Empty<Permission>() : permissions
                .ToList()
                .AsReadOnly();
        }

        public string Name { get; private set; }
        public IReadOnlyList<Permission> Permissions { get; private set; }
    }
}
