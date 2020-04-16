using System.Security.Claims;
using System.Security.Principal;

namespace Odachi.Testing.Scenarios
{
	public class ApplicationPrincipalHolder
	{
		public IPrincipal Principal { get; set; } = new ClaimsPrincipal();
	}
}
