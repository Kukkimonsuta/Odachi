using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Description;
using Odachi.Extensions.Reflection;
using Odachi.JsonRpc.Server.Internal;
using Odachi.JsonRpc.Server.Model;

namespace Odachi.CodeModel.Providers.JsonRpc.Description
{
	public class JsonRpcMethodDescriptor : IMethodDescriptor
	{
		protected virtual void DescribeJsonRpcMethod(MethodBuilder builder, JsonRpcMethod jsonRpcMethod)
		{
			builder.Hint("jsonrpc-name", jsonRpcMethod.Name);
			builder.Hint("jsonrpc-module-name", jsonRpcMethod.ModuleName);
			builder.Hint("jsonrpc-method-name", jsonRpcMethod.MethodName);
		}

		public void Describe(MethodBuilder builder)
		{
			switch (builder.Source)
			{
				case JsonRpcMethod jsonRpcMethod:
					DescribeJsonRpcMethod(builder, jsonRpcMethod);
					break;
			}
		}
	}
}
