using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Odachi.Abstractions;
using Odachi.Annotations;
using Odachi.AspNetCore.JsonRpc.Internal;
using Odachi.JsonRpc.Common;

namespace JsonRpcSample
{
	public class StorageModule
	{
		[RpcMethod]
		public async Task<IEnumerable<string>> UploadAsync(IBlob file)
		{
			if (file == null)
				return new[] { "no-file" };

			using (var stream = await file.OpenReadAsync())
			{
				return new[]
				{
					file.Name,
					stream.Length.ToString()
				};
			}
		}
	}

	public class ErrorModule
	{
		[RpcMethod]
		public int UnauthorizedMethod()
		{
			throw new SecurityException("Unauthorized method");
		}

		[RpcMethod]
		public void UnauthorizedNotification()
		{
			throw new SecurityException("Unauthorized notification");
		}
	}

	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<ErrorModule>();
			services.AddScoped<StorageModule>();

			services.AddJsonRpc(options =>
			{
				options.Methods.AddReflected<ErrorModule>();
				options.Methods.AddReflected<StorageModule>();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseStatusCodePages();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.Map("/api", rpc => rpc.UseJsonRpc());
			app.Run(context =>
			{
				context.Response.StatusCode = StatusCodes.Status200OK;
				context.Response.ContentType = "text/html";

				return context.Response.SendFileAsync(env.WebRootFileProvider.GetFileInfo("index.html"));
			});
		}
	}
}
