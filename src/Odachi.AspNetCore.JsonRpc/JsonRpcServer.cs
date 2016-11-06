using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.AspNetCore.JsonRpc.Internal;
using Odachi.AspNetCore.JsonRpc.Behaviors;

namespace Odachi.AspNetCore.JsonRpc
{
	public class JsonRpcServer
	{
		public JsonRpcServer(JsonRpcMethodCollection methods, JsonRpcBehaviorCollection behaviors)
		{
			if (methods == null)
				throw new ArgumentNullException(nameof(methods));
			if (behaviors == null)
				throw new ArgumentNullException(nameof(behaviors));

			Methods = methods;
			Behaviors = behaviors;
		}

		public JsonRpcMethodCollection Methods { get; }
		public JsonRpcBehaviorCollection Behaviors { get; }

		public async Task ProcessAsync(JsonRpcContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			try
			{
				var request = context.Request;

				JsonRpcMethod method;

				if (!Methods.TryGetMethod(request.Method, out method))
				{
					context.SetResponse(JsonRpcError.METHOD_NOT_FOUND);
					return;
				}

				for (var i = 0; i < Behaviors.Count; i++)
					await Behaviors[i].BeforeInvoke(context);

				await method.HandleAsync(context);

				for (var i = 0; i < Behaviors.Count; i++)
					await Behaviors[i].AfterInvoke(context);
			}
			catch (JsonRpcException ex)
			{
				// todo: log

				context.SetResponse(ex.JsonRpcCode, ex.JsonRpcMessage, data: ex.JsonRpcData);
				return;
			}
			catch (Exception ex)
			{
				// todo: log

				context.SetResponse(JsonRpcError.INTERNAL_ERROR, data: ex.ToString());
				return;
			}
		}
	}
}
