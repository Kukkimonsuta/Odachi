using System;

namespace Odachi.JsonRpc.Client
{
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

		public object Id { get; }
		public string Method { get; }
		public object Params { get; }

		public bool IsNotification => Id == null;
	}
}
