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
using System.Threading;
using System.Security;
using System.Reflection;
using Odachi.JsonRpc.Common;
using Odachi.JsonRpc.Common.Converters;
using Odachi.Abstractions;
using System.Net.Http;
using Odachi.JsonRpc.Server;
using Odachi.JsonRpc.Server.Internal;
using Odachi.JsonRpc.Common.Internal;
using Odachi.JsonRpc.Server.Builder;

namespace Odachi.AspNetCore.JsonRpc
{
	public class JsonRpcMiddleware
	{
		public JsonRpcMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IOptions<JsonRpcOptions> options)
		{
			_logger = loggerFactory.CreateLogger<JsonRpcMiddleware>();
			_next = next;
			_options = options.Value;

			var handler = new JsonRpcHandlerBuilder()
				.UseSecurityExceptionHandler()
				.UseBlobHandler()
				.Build();

			_server = new JsonRpcServer(options.Value, handler, loggerFactory.CreateLogger<JsonRpcServer>());
		}

		private readonly ILogger _logger;
		private readonly RequestDelegate _next;
		private readonly JsonRpcOptions _options;
		private readonly JsonRpcServer _server;

		private async Task SendResponse(HttpContext httpContext, JsonRpcResponse response)
		{
			var jObject = JObject.FromObject(response, _server.Serializer);

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
			await using (var writer = new StreamWriter(httpContext.Response.Body))
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				await jObject.WriteToAsync(jsonWriter);

				await jsonWriter.FlushAsync();
				await writer.FlushAsync();

				await jsonWriter.CloseAsync();
			}
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				JsonRpcRequest request;
				try
				{
					request = await CreateRequestAsync(httpContext, _server.Serializer);
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

				var response = await _server.ProcessAsync(httpContext.RequestServices, request);

				if (request.IsNotification)
				{
					httpContext.Response.StatusCode = 204;
				}
				else
				{
					await SendResponse(httpContext, response);
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

		#region Static members

		private static async Task<JsonReader> CreateRequestReaderAsync(HttpContext httpContext, JsonSerializer serializer)
		{
			if (!httpContext.Request.HasFormContentType)
			{
				return new JsonTextReader(new StreamReader(httpContext.Request.Body));
			}

			var form = await httpContext.Request.ReadFormAsync();

			return new JsonTextReader(new StringReader(form.Single().Value));
		}

		private static async Task<JsonRpcRequest> CreateRequestAsync(HttpContext httpContext, JsonSerializer serializer)
		{
			using (var reader = await CreateRequestReaderAsync(httpContext, serializer))
			{
				var requestJsonToken = await JToken.ReadFromAsync(reader);
				if (requestJsonToken.Type != JTokenType.Object)
					throw new JsonRpcException(JsonRpcError.INVALID_REQUEST, "Invalid request (wrong root type)");

				var requestJson = (JObject)requestJsonToken;

				object id = null;
				if (requestJson.TryGetValue("id", out JToken idJson))
				{
					switch (idJson.Type)
					{
						case JTokenType.Null:
							break;

						case JTokenType.String:
							id = idJson.Value<string>();
							break;

						case JTokenType.Integer:
							id = idJson.Value<int>();
							break;

						default:
							throw new JsonRpcException(JsonRpcError.INVALID_REQUEST, "Invalid request (wrong id type)");
					}
				}

				if (!requestJson.TryGetValue("method", out JToken methodJson) || methodJson.Type != JTokenType.String)
					throw new JsonRpcException(JsonRpcError.INVALID_REQUEST, "Invalid 'method' (missing or wrong type)");
				var method = methodJson.Value<string>();

				if (!requestJson.TryGetValue("params", out JToken paramsJson))
					paramsJson = null;
				if (paramsJson != null && paramsJson.Type != JTokenType.Object && paramsJson.Type != JTokenType.Array)
					throw new JsonRpcException(JsonRpcError.INVALID_REQUEST, "Invalid 'params' (wrong type)");

				return new JsonRpcRequest(id, method, paramsJson, serializer);
			}
		}

		#endregion
	}
}
