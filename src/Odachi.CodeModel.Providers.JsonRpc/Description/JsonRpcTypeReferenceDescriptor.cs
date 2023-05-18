using System.Threading.Tasks;
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
			var index = builder.SourceIndex;

			// when method returns Task, the generic index is off by one because codemodel otherwise ignores it
			var returnType = reflectedJsonRpcMethod.Method.ReturnType;
			if (returnType.IsGenericType && (returnType.GetGenericTypeDefinition() == typeof(Task<>) || returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
			{
				index += 1;
			}

			if (index == 0)
			{
				if (reflectedJsonRpcMethod.Method.ReturnParameter?.IsNonNullable() == true)
				{
					builder.IsNullable = false;
				}
			}
			else
			{
				index -= 1;

				if (reflectedJsonRpcMethod.Method.ReturnParameter?.IsGenericArgumentNonNullable(index) == true)
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
