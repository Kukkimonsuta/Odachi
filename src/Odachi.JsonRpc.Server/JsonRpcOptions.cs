using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Odachi.JsonRpc.Common.Converters;
using Odachi.JsonRpc.Common.Internal;
using Odachi.JsonRpc.Server.Model;
using Odachi.JsonRpc.Server.Modules;

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
			JsonSerializerSettings.Converters.Add(new DateOnlyConverter());
			JsonSerializerSettings.Converters.Add(new NullableDateOnlyConverter());
			JsonSerializerSettings.Converters.Add(new TimeOnlyConverter());
			JsonSerializerSettings.Converters.Add(new NullableTimeOnlyConverter());
			JsonSerializerSettings.Converters.Add(new TimeSpanConverter());
			JsonSerializerSettings.Converters.Add(new NullableTimeSpanConverter());
			JsonSerializerSettings.Converters.Add(new PageConverter());
			JsonSerializerSettings.Converters.Add(new BlobConverter());

			Methods.AddReflected<ServerModule>();
		}

		/// <summary>
		/// Include "jsonrpc: 2.0" constant in responses.
		/// </summary>
		public bool UseJsonRpcConstant { get; set; } = false;
		/// <summary>
		/// Include error data in error responses.
		/// </summary>
		public bool AllowErrorData { get; set; } = true;
		/// <summary>
		/// Allow only http post.
		/// </summary>
		public bool ForceHttpPost { get; set; } = true;
        /// <summary>
        /// Allow setting http status based on RPC response.
        /// </summary>
        public bool AllowHttpErrorStatus { get; set; } = true;

		public JsonSerializerSettings JsonSerializerSettings { get; set; }

		public JsonRpcMethodCollection Methods { get; } = new JsonRpcMethodCollection();
	}
}
