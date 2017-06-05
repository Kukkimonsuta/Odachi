using System;
using Newtonsoft.Json.Linq;

namespace Odachi.JsonRpc.Client
{
	public class JsonRpcResponse
	{
		public JsonRpcResponse(JToken id, JToken result, JToken error)
		{
			if (result != null && id == null)
				throw new ArgumentNullException(nameof(id));
			if (result != null && error != null)
				throw new ArgumentException("JsonRpcResponse cannot have both 'result' and 'error'");

			Id = id;
			Result = result;
			Error = error;
		}

		public JToken Id { get; }
		public JToken Result { get; }
		public JToken Error { get; }
	}
}
