using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class JsonRpcContext
	{
		public JsonRpcContext(IServiceProvider requestServices, JsonRpcServer server, JsonRpcRequest request)
		{
			if (requestServices == null)
				throw new ArgumentNullException(nameof(requestServices));
			if (server == null)
				throw new ArgumentNullException(nameof(server));
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			RequestServices = requestServices;
			Server = server;
			Request = request;
		}

		public IServiceProvider RequestServices { get; }
		public JsonRpcServer Server { get; }
		public JsonRpcRequest Request { get; }
		public JsonRpcResponse Response { get; protected set; }

		public void SetResponse(object result)
		{
			if (Request.IsNotification)
				return;

			Response = new JsonRpcResponse(Request.Id, result);
		}
		public void SetResponse(int errorCode, object data = null)
		{
			if (Request.IsNotification)
				return;

			Response = new JsonRpcResponse(Request.Id, errorCode, errorData: data);
		}
		public void SetResponse(int errorCode, string errorMessage, object data = null)
		{
			if (Request.IsNotification)
				return;

			Response = new JsonRpcResponse(Request.Id, errorCode, errorMessage, errorData: data);
		}
	}
}
