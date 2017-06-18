using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.JsonRpc.Common
{
	/// <summary>
	/// Describes error codes used within json rpc protocol.
	/// </summary>
	public class JsonRpcError
	{
		/// <summary>
		/// Invalid JSON was received by the server.
		/// An error occurred on the server while parsing the JSON text.
		/// </summary>
		public const int PARSE_ERROR = -32700;
		/// <summary>
		/// The JSON sent is not a valid Request object.
		/// </summary>
		public const int INVALID_REQUEST = -32600;
		/// <summary>
		/// The method does not exist / is not available.
		/// </summary>
		public const int METHOD_NOT_FOUND = -32601;
		/// <summary>
		/// Invalid method parameter(s).
		/// </summary>
		public const int INVALID_PARAMS = -32602;
		/// <summary>
		/// Internal JSON-RPC error.
		/// </summary>
		public const int INTERNAL_ERROR = -32603;

		// -32000 to -32099	Server error reserved for implementation-defined server-errors.

		/// <summary>
		/// The server method doesn't return any value and is expected to be only called as notification.
		/// </summary>
		public const int NOTIFICATION_EXPECTED = -32000;

		/// <summary>
		/// Client is not authorized to call given method (authenticate and try again).
		/// </summary>
		public const int UNAUTHORIZED = -32001;

		/// <summary>
		/// Client is not allowed to call given method (display error).
		/// </summary>
		public const int FORBIDDEN = -32002;

		/// <summary>
		/// Returns message for given error code.
		/// </summary>
		public static string GetMessage(int code)
		{
			switch (code)
			{
				case PARSE_ERROR:
					return "Parse error";

				case INVALID_REQUEST:
					return "Invalid request";

				case METHOD_NOT_FOUND:
					return "Method not found";

				case INVALID_PARAMS:
					return "Invalid params";

				case INTERNAL_ERROR:
					return "Internal error";

				case NOTIFICATION_EXPECTED:
					return "Notification expected";

				case UNAUTHORIZED:
					return "Unauthorized";

				case FORBIDDEN:
					return "Forbidden";

				default:
					throw new InvalidOperationException($"Undefined behavior for code '{code}'");
			}
		}
	}
}
