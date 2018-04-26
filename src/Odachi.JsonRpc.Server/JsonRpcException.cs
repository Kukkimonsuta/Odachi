using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.JsonRpc.Common;

namespace Odachi.JsonRpc.Server
{
	public class JsonRpcException : Exception
	{
		public JsonRpcException(int code, object data = null)
			: base(JsonRpcError.GetMessage(code))
		{
			JsonRpcCode = code;
			JsonRpcData = data;
		}
		public JsonRpcException(int code, string message, object data = null)
			: base(message)
		{
			JsonRpcCode = code;
			JsonRpcData = data;
		}

		public int JsonRpcCode { get; }
		public string JsonRpcMessage => Message;
		public object JsonRpcData { get; }
	}
}
