using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Odachi.JsonRpc.Common.Converters;
using Odachi.JsonRpc.Common.Internal;
using Odachi.JsonRpc.Server.Internal;
using Odachi.JsonRpc.Server.Model;
using Odachi.JsonRpc.Server.Modules;
using Odachi.JsonRpc.Server.Builder;
using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Odachi.JsonRpc.Common;

namespace Odachi.JsonRpc.Server
{
	public class JsonRpcOptions
	{
		public JsonRpcOptions()
		{
			JsonSerializerSettings = new JsonSerializerSettings()
			{
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new MultiWordCamelCaseNamingStrategy(true, false)
				},
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.None,
			};
			JsonSerializerSettings.Converters.Add(new PageConverter());
			JsonSerializerSettings.Converters.Add(new BlobConverter());
#pragma warning disable CS0618 // Type or member is obsolete
			JsonSerializerSettings.Converters.Add(new EntityReferenceConverter());
			JsonSerializerSettings.Converters.Add(new StreamReferenceConverter());
#pragma warning restore CS0618 // Type or member is obsolete

			Methods.AddReflected<ServerModule>();
		}

		public bool UseJsonRpcConstant { get; set; } = false;

		public JsonSerializerSettings JsonSerializerSettings { get; set; }

		public JsonRpcMethodCollection Methods { get; } = new JsonRpcMethodCollection();
	}
}
