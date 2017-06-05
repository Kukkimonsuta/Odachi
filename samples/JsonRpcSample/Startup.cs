﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Odachi.Abstractions;
using Odachi.Annotations;

namespace JsonRpcSample
{
	public class StorageModule
	{
		[RpcMethod]
		public IEnumerable<string> Upload(IStreamReference file)
		{
			if (file == null)
				return new[] { "no-file" };

			using (var stream = file.OpenReadStream())
			{
				return new[]
				{
					file.Name,
					stream.Length.ToString()
				};
			}
		}
	}

	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<StorageModule>();

			services.AddJsonRpc(options =>
			{
				options.Methods.AddReflected<StorageModule>();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();

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
