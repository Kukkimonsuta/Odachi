using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Odachi.AspNetCore.JsonRpc.Behaviors;
using Odachi.AspNetCore.JsonRpc.Converters;
using Odachi.AspNetCore.JsonRpc.Model;
using Odachi.AspNetCore.JsonRpc.Modules;
using Odachi.JsonRpc.Common.Converters;
using Odachi.JsonRpc.Common.Internal;

namespace Odachi.AspNetCore.JsonRpc
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
			JsonSerializerSettings.Converters.Add(new EntityReferenceConverter());

			Behaviors.Add(new SecurityErrorBehavior());

			Methods.AddReflected<ServerModule>();
		}

		public bool UseJsonRpcConstant { get; set; } = false;

		public JsonSerializerSettings JsonSerializerSettings { get; set; }

		public JsonRpcBehaviorCollection Behaviors { get; } = new JsonRpcBehaviorCollection();

		public JsonRpcMethodCollection Methods { get; } = new JsonRpcMethodCollection();
	}
}
