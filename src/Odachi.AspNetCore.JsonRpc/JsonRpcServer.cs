using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.AspNetCore.JsonRpc.Internal;
using Odachi.AspNetCore.JsonRpc.Behaviors;
using Microsoft.Extensions.Logging;
using Odachi.AspNetCore.JsonRpc.Model;

namespace Odachi.AspNetCore.JsonRpc
{
	public class JsonRpcServer
	{
		public JsonRpcServer(ILoggerFactory loggerFactory, JsonRpcMethodCollection methods, JsonRpcBehaviorCollection behaviors)
		{
			if (methods == null)
				throw new ArgumentNullException(nameof(methods));
			if (behaviors == null)
				throw new ArgumentNullException(nameof(behaviors));

			_logger = loggerFactory.CreateLogger<JsonRpcServer>();
			Methods = methods;
			Behaviors = behaviors;
		}

		private readonly ILogger _logger;

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
				{
					await Behaviors[i].BeforeInvoke(context);
				}

				await method.HandleAsync(context);

				for (var i = 0; i < Behaviors.Count; i++)
				{
					await Behaviors[i].AfterInvoke(context);
				}
			}
			catch (JsonRpcException ex)
			{
				if (ex.JsonRpcCode == JsonRpcError.INTERNAL_ERROR)
				{
					_logger.LogError(JsonRpcLogEvents.InternalError, ex, "JsonRpc call failed");
				}
				else
				{
					_logger.LogWarning(JsonRpcLogEvents.GenericError, ex, "JsonRpc call failed");
				}

				context.SetResponse(ex.JsonRpcCode, ex.JsonRpcMessage, data: ex.JsonRpcData);
				return;
			}
			catch (Exception ex)
			{
				_logger.LogError(JsonRpcLogEvents.InternalError, ex, "JsonRpc call crashed");

				context.SetResponse(JsonRpcError.INTERNAL_ERROR, data: ex.ToString());
				return;
			}
		}
	}
}
