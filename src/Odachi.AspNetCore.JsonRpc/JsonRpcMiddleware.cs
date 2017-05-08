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

namespace Odachi.AspNetCore.JsonRpc
{
	public class CompositeServiceProvider : IServiceProvider
	{
		public CompositeServiceProvider(params IServiceProvider[] providers)
		{
			if (providers == null)
				throw new ArgumentNullException(nameof(providers));

			_providers = providers;
		}

		private IServiceProvider[] _providers;

		public object GetService(Type serviceType)
		{
			// todo: handle many

			// the service in last-added provider wins
			for (var i = _providers.Length - 1; i >= 0; i--)
			{
				var provider = _providers[i];
				var service = provider.GetService(serviceType);

				if (service != null)
				{
					return service;
				}
			}

			return null;
		}
	}

	public class JsonRpcMiddleware
	{
		public JsonRpcMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<JsonRpcOptions> options)
		{
			_logger = loggerFactory.CreateLogger<JsonRpcMiddleware>();
			_next = next;
			_options = options.Value;
			_server = new JsonRpcServer(loggerFactory, _options.Methods, _options.Behaviors);

			var container = new ServiceCollection();
			container.AddSingleton(_server);
			foreach (var behavior in _options.Behaviors)
				behavior.ConfigureServices(container);
			_rpcServices = container.BuildServiceProvider();
		}

		private readonly ILogger _logger;
		private readonly RequestDelegate _next;
		private readonly JsonRpcOptions _options;
		private readonly JsonRpcServer _server;
		private readonly IServiceProvider _rpcServices;

		private async Task SendResponse(HttpContext httpContext, JsonRpcResponse response, JsonSerializer serializer)
		{
			httpContext.Response.StatusCode = 200;
			httpContext.Response.ContentType = "application/json";

			using (var writer = new StreamWriter(httpContext.Response.Body))
			{
				var jObject = JObject.FromObject(response, serializer);

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

				// write to output
				using (var jsonWriter = new JsonTextWriter(writer))
				{
					jObject.WriteTo(jsonWriter);
				}

				await writer.FlushAsync();
			}
		}

		public async Task Invoke(HttpContext httpContext)
		{
			// serializer may be mutated in JsonRpcRequest.CreateAsync, so we need new instance for every request
			var serializer = JsonSerializer.Create(_options.JsonSerializerSettings);
			try
			{
				using (var scope = _rpcServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					var requestServices = new CompositeServiceProvider(
						httpContext.RequestServices,
						scope.ServiceProvider
					);

					JsonRpcRequest request;
					try
					{
						request = await JsonRpcRequest.CreateAsync(httpContext, serializer);
					}
					catch (Exception ex)
					{
						_logger.LogWarning(JsonRpcLogEvents.ParseError, ex, "Failed to parse request");

						if (!httpContext.Response.HasStarted && !httpContext.RequestAborted.IsCancellationRequested)
                        {
                            await SendResponse(httpContext, new JsonRpcResponse(null, JsonRpcError.PARSE_ERROR, errorData: ex.ToString()), serializer);
                        }
						return;
					}

					var rpcContext = new JsonRpcContext(requestServices, _server, request);

					await _server.ProcessAsync(rpcContext);

					if (rpcContext.Request.IsNotification)
					{
						httpContext.Response.StatusCode = 204;
					}
					else
					{
						await SendResponse(httpContext, rpcContext.Response, serializer);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(JsonRpcLogEvents.InternalError, ex, "Failed to send response");

				if (!httpContext.Response.HasStarted && !httpContext.RequestAborted.IsCancellationRequested)
                {
                    await SendResponse(httpContext, new JsonRpcResponse(null, JsonRpcError.INTERNAL_ERROR, errorData: ex.ToString()), serializer);
                }
			}
		}
	}
}
