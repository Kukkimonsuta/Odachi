#nullable enable

using System.Threading.Tasks;
using Odachi.Extensions.Primitives;

namespace Odachi.CodeModel.Providers.JsonRpc.Tests.Model
{
	public class NrtOnService
	{
		public Task<OneOf<string, int>> TestAsync()
		{
			return Task.FromResult<OneOf<string, int>>(0);
		}

		public OneOf<string, int> Test()
		{
			return 0;
		}

		public Task<OneOf<string?, int>> TestNullableAsync()
		{
			return Task.FromResult<OneOf<string?, int>>(0);
		}

		public OneOf<string?, int> TestNullable()
		{
			return 0;
		}
	}
}
