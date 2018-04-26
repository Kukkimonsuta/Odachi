using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.JsonRpc.Server.Model;

namespace Odachi.JsonRpc.Server
{
	public class JsonRpcContext
	{
		public JsonRpcContext(JsonRpcServer server, JsonRpcRequest request, IServiceProvider appServices)
		{
			Server = server ?? throw new ArgumentNullException(nameof(server));
			Request = request ?? throw new ArgumentNullException(nameof(request));
			AppServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
		}

		public JsonRpcServer Server { get; }
		public JsonRpcRequest Request { get; }
		public JsonRpcMethod Method { get; set; }
		public JsonRpcResponse Response { get; protected set; }

		public IServiceProvider AppServices { get; }

		public bool WasHandled { get; set; }

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
