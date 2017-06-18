using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Odachi.Abstractions;
using Odachi.AspNetCore.JsonRpc.Converters;

namespace Odachi.JsonRpc.Client
{
	[DataContract]
	public class JsonRpcRequest
	{
		public JsonRpcRequest(object id, string method, object @params)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			Id = id;
			Method = method;
			Params = @params;
		}

		[DataMember]
		public object Id { get; }
		[DataMember]
		public string Method { get; }
		[DataMember]
		public object Params { get; }

		public bool IsNotification => Id == null;

		#region Static members

		private static int NextId = 1;

		public static JsonRpcRequest Create(string method, object @params)
		{
			return new JsonRpcRequest(Interlocked.Increment(ref NextId), method, @params);
		}

		#endregion
	}
}
