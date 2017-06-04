using Odachi.AspNetCore.JsonRpc.Model;
using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Description;

namespace Odachi.CodeModel.Providers.JsonRpc.Description
{
	public class JsonRpcMethodDescriptor : IMethodDescriptor
	{
		protected virtual void DescribeJsonRpcMethod(MethodBuilder builder, JsonRpcMethod jsonRpcMethod)
		{
			builder.Hint("jsonrpc-method-name", jsonRpcMethod.Name);
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
