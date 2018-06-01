using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Odachi.JsonRpc.Common;
using System.Diagnostics;
using Odachi.JsonRpc.Server.Internal;
using Odachi.JsonRpc.Server.Model;
using Newtonsoft.Json;
using Odachi.JsonRpc.Common.Internal;

namespace Odachi.JsonRpc.Server
{
	public class JsonRpcServer
	{
		private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

		public JsonRpcServer(JsonRpcOptions options, JsonRpcRequestDelegate handler, ILogger<JsonRpcServer> logger)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_handler = handler ?? throw new ArgumentNullException(nameof(handler));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			Methods = options.Methods;

			Serializer = JsonSerializer.Create(_options.JsonSerializerSettings);
		}

		private readonly JsonRpcRequestDelegate _handler;
		private readonly JsonRpcOptions _options;
		private readonly ILogger _logger;

		public JsonSerializer Serializer { get; }
		public JsonRpcMethodCollection Methods { get; }

		public async Task<JsonRpcResponse> ProcessAsync(IServiceProvider appServices, JsonRpcRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			using (_logger.BeginScope(new Dictionary<string, object>()
			{
				["JsonRpcRequestId"] = request.Id,
				["JsonRpcRequestMethod"] = request.Method,
			}))
			{
				_logger.LogInformation(JsonRpcLogEvents.RequestStarting, "Rpc request {Method} starting", request.Method);
				var startTimestamp = Stopwatch.GetTimestamp();

				var context = new JsonRpcContext(this, request, appServices);
				try
				{
					if (Methods.TryGetMethod(request.Method, out var method))
					{
						context.Method = method;
					}

					await _handler(context);

					if (!context.Request.IsNotification && context.Response == null)
					{
						throw new JsonRpcException(JsonRpcError.INTERNAL_ERROR, "Handler failed to produce a response");
					}

					var elapsed = new TimeSpan((long)(TimestampToTicks * (Stopwatch.GetTimestamp() - startTimestamp)));
					_logger.LogInformation(JsonRpcLogEvents.RequestFinished, "Rpc request {Method} finished in {ElapsedMilliseconds}ms", request.Method, elapsed.TotalMilliseconds);
				}
				catch (Exception ex)
				{
					var elapsed = new TimeSpan((long)(TimestampToTicks * (Stopwatch.GetTimestamp() - startTimestamp)));

					var jsonRpcException = ex.Unwrap<JsonRpcException>();
					if (jsonRpcException != null)
					{
						if (jsonRpcException.JsonRpcCode == JsonRpcError.INTERNAL_ERROR)
						{
							_logger.LogError(JsonRpcLogEvents.InternalError, jsonRpcException, "Rpc request {Method} failed after {ElapsedMilliseconds}ms", request.Method, elapsed.TotalMilliseconds);
						}
						else
						{
							_logger.LogWarning(JsonRpcLogEvents.GenericError, jsonRpcException, "Rpc request {Method} failed after {ElapsedMilliseconds}ms", request.Method, elapsed.TotalMilliseconds);
						}

						context.SetResponse(jsonRpcException.JsonRpcCode, jsonRpcException.JsonRpcMessage, data: jsonRpcException.JsonRpcData);
						return context.Response;
					}

					_logger.LogError(JsonRpcLogEvents.InternalError, ex, "Rpc request {Method} crashed after {ElapsedMilliseconds}ms", request.Method, elapsed.TotalMilliseconds);

					context.SetResponse(JsonRpcError.INTERNAL_ERROR, data: ex.Unwrap().ToDiagnosticString());
					return context.Response;
				}

				return context.Response;
			}
		}
	}
}
