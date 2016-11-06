using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.AspNetCore.JsonRpc.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Collections;
using Odachi.AspNetCore.JsonRpc.Modules;
using Odachi.AspNetCore.JsonRpc.Behaviors;

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
					NamingStrategy = new CamelCaseNamingStrategy()
				},
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.None,
			};
		}

		public bool UseJsonRpcConstant { get; set; } = false;

		public JsonSerializerSettings JsonSerializerSettings { get; set; }

		public JsonRpcBehaviorCollection Behaviors { get; } = new JsonRpcBehaviorCollection();

		public JsonRpcMethodCollection Methods { get; } = new JsonRpcMethodCollection();
	}
}
