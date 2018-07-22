using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.JsonRpc.Server
{
	public delegate Task JsonRpcRequestDelegate(JsonRpcContext context);
}
