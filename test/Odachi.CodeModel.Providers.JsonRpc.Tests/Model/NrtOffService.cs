#nullable disable

using System.Threading.Tasks;
using Odachi.Extensions.Primitives;

namespace Odachi.CodeModel.Providers.JsonRpc.Tests.Model
{
	public class NrtOffService
	{
		public Task<OneOf<string, int>> TestAsync()
		{
			return Task.FromResult<OneOf<string, int>>(0);
		}

		public OneOf<string, int> Test()
		{
			return 0;
		}
	}
}
