using System;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.JsonRpc.Server.Builder
{
	public interface IJsonRpcHandlerBuilder
	{
		IJsonRpcHandlerBuilder Use(Func<JsonRpcRequestDelegate, JsonRpcRequestDelegate> middleware);

		JsonRpcRequestDelegate Build();
	}
}
