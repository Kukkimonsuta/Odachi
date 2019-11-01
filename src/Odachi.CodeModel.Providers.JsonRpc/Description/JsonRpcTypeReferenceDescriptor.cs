using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Description;
using Odachi.Extensions.Reflection;
using Odachi.JsonRpc.Server.Internal;
using Odachi.JsonRpc.Server.Model;

namespace Odachi.CodeModel.Providers.JsonRpc.Description
{
	public class JsonRpcTypeReferenceDescriptor : ITypeReferenceDescriptor
	{
		protected virtual void DescribeJsonRpcMethod(TypeReferenceBuilder builder, ReflectedJsonRpcMethod reflectedJsonRpcMethod)
		{
			if (builder.SourceIndex == 0)
			{
				if (reflectedJsonRpcMethod.Method.ReturnParameter?.IsNonNullable() == true)
				{
					builder.IsNullable = false;
				}
			}
			else
			{
				if (reflectedJsonRpcMethod.Method.ReturnParameter?.IsGenericArgumentNonNullable(builder.SourceIndex - 1) == true)
				{
					builder.IsNullable = false;
				}
			}
		}

		protected virtual void DescribeJsonRpcParameter(TypeReferenceBuilder builder, ReflectedJsonRpcParameter reflectedJsonRpcParameter)
		{
			if (builder.SourceIndex == 0)
			{
				if (reflectedJsonRpcParameter.Parameter.IsNonNullable())
				{
					builder.IsNullable = false;
				}
			}
			else
			{
				if (reflectedJsonRpcParameter.Parameter.IsGenericArgumentNonNullable(builder.SourceIndex - 1))
				{
					builder.IsNullable = false;
				}
			}
		}

		public void Describe(TypeReferenceBuilder builder)
		{
			switch (builder.Source)
			{
				case ReflectedJsonRpcMethod reflectedJsonRpcMethod:
					DescribeJsonRpcMethod(builder, reflectedJsonRpcMethod);
					break;

				case ReflectedJsonRpcParameter reflectedJsonRpcParameter:
					DescribeJsonRpcParameter(builder, reflectedJsonRpcParameter);
					break;
			}
		}
	}
}
