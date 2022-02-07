#nullable enable

namespace Odachi.Security
{
	/// <summary>
	/// Class representing a permission.
	/// </summary>
	public sealed class Permission : ClaimTemplate
	{
		public const string PermissionClaim = "http://schemas.algorim.com/2015/09/odachi/claims/permission";

		public Permission(string template)
			: base(template)
		{
		}
	}
}
