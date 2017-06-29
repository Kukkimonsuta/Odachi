using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Options;
using Odachi.AspNetCore.JsonRpc.Internal;
using Microsoft.Extensions.Logging;
using Odachi.AspNetCore.JsonRpc.Modules;
using System.Threading;
using System.Security;
using System.Reflection;
using Odachi.JsonRpc.Common;
using Odachi.JsonRpc.Common.Converters;
using Odachi.Abstractions;
using System.Net.Http;

namespace Odachi.AspNetCore.JsonRpc
{
	public class JsonRpcMiddleware
	{
		public JsonRpcMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IOptions<JsonRpcOptions> options)
		{
			_logger = loggerFactory.CreateLogger<JsonRpcMiddleware>();
			_next = next;
			_options = options.Value;
			_server = new JsonRpcServer(loggerFactory, _options.Methods, _options.Behaviors);

			_serializer = JsonSerializer.Create(_options.JsonSerializerSettings);

			var container = new ServiceCollection();
			container.AddSingleton(_server);
			container.AddSingleton(_serializer);
			foreach (var behavior in _options.Behaviors)
				behavior.ConfigureRpcServices(container);
			_rpcServices = container.BuildServiceProvider();

			var internalTypes = container.Select(d => d.ServiceType).ToArray();
			foreach (var method in _server.Methods)
			{
				method.Analyze(_server, internalTypes);
			}
		}

		private readonly ILogger _logger;
		private readonly RequestDelegate _next;
		private readonly JsonRpcOptions _options;
		private readonly JsonRpcServer _server;
		private readonly IServiceProvider _rpcServices;
		private readonly JsonSerializer _serializer;

		private async Task SendResponse(HttpContext httpContext, JsonRpcResponse response)
		{
			var jObject = JObject.FromObject(response, _serializer);

			// append jsonrpc constant if enabled
			if (_options.UseJsonRpcConstant)
			{
				jObject.AddFirst(new JProperty("jsonrpc", "2.0"));
			}

			// ensure that respose has either result or error property
			if (jObject.Property("result") == null && jObject.Property("error") == null)
			{
				jObject.Add("result", null);
			}

			httpContext.Response.StatusCode = 200;
			httpContext.Response.ContentType = "application/json";
			using (var writer = new StreamWriter(httpContext.Response.Body))
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				await jObject.WriteToAsync(jsonWriter);

				await jsonWriter.FlushAsync();
				await writer.FlushAsync();
			}
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				using (var scope = _rpcServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					JsonRpcRequest request;
					try
					{
						request = await JsonRpcRequest.CreateAsync(httpContext, _serializer);
					}
					catch (Exception ex)
					{
						_logger.LogWarning(JsonRpcLogEvents.ParseError, ex, "Failed to parse request");

						if (!httpContext.Response.HasStarted && !httpContext.RequestAborted.IsCancellationRequested)
						{
							await SendResponse(httpContext, new JsonRpcResponse(null, JsonRpcError.PARSE_ERROR, errorData: ex.ToString()));
						}
						return;
					}

					var rpcContext = new JsonRpcContext(httpContext.RequestServices, scope.ServiceProvider, _server, request);

					await _server.ProcessAsync(rpcContext);

					if (rpcContext.Request.IsNotification)
					{
						httpContext.Response.StatusCode = 204;
					}
					else
					{
						await SendResponse(httpContext, rpcContext.Response);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(JsonRpcLogEvents.InternalError, ex, "Failed to send response");

				if (!httpContext.Response.HasStarted && !httpContext.RequestAborted.IsCancellationRequested)
				{
					await SendResponse(httpContext, new JsonRpcResponse(null, JsonRpcError.INTERNAL_ERROR, errorData: ex.Unwrap().ToDiagnosticString()));
				}
			}
		}
	}
}
