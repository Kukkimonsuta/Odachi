using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class JsonRpcContext
	{
		public JsonRpcContext(IServiceProvider appServices, IServiceProvider rpcServices, JsonRpcServer server, JsonRpcRequest request)
		{
			if (appServices == null)
				throw new ArgumentNullException(nameof(appServices));
			if (server == null)
				throw new ArgumentNullException(nameof(server));
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			AppServices = appServices;
			RpcServices = rpcServices;
			Server = server;
			Request = request;
		}

		public IServiceProvider AppServices { get; }
		public IServiceProvider RpcServices { get; }
		public JsonRpcServer Server { get; }
		public JsonRpcRequest Request { get; }
		public JsonRpcResponse Response { get; protected set; }

		public bool WasHandled { get; protected set; }

		public void SetResponse(object result, bool handled = true)
		{
			if (handled)
			{
				WasHandled = true;
			}

			if (Request.IsNotification)
				return;

			Response = new JsonRpcResponse(Request.Id, result);
		}
		public void SetResponse(int errorCode, object data = null, bool handled = true)
		{
			if (handled)
			{
				WasHandled = true;
			}

			if (Request.IsNotification)
				return;

			Response = new JsonRpcResponse(Request.Id, errorCode, errorData: data);
		}
		public void SetResponse(int errorCode, string errorMessage, object data = null, bool handled = true)
		{
			if (handled)
			{
				WasHandled = true;
			}

			if (Request.IsNotification)
				return;

			Response = new JsonRpcResponse(Request.Id, errorCode, errorMessage, errorData: data);
		}
	}
}
