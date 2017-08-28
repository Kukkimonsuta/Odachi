using Odachi.CodeModel.Description;
using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class MethodBuilder : BuilderBase<MethodBuilder, MethodFragment>
	{
		public MethodBuilder(PackageContext context, string name, ITypeReference returnType, object source)
			: base(context, name)
		{
			if (returnType == null)
				throw new ArgumentNullException(nameof(returnType));

			ReturnType = returnType;
			Source = source;

			Context.MethodDescriptors.Describe(this);
		}

		public IList<ParameterFragmentBuilder> Parameters { get; } = new List<ParameterFragmentBuilder>();
		public ITypeReference ReturnType { get; }
		public object Source { get; }

		public MethodBuilder Parameter(string name, ITypeReference type, object source, Action<ParameterFragmentBuilder> configure = null)
		{
			var parameterBuilder = new ParameterFragmentBuilder(Context, name, type, source);

			configure?.Invoke(parameterBuilder);

			Parameters.Add(parameterBuilder);

			return this;
		}

		public override MethodFragment Build()
		{
			var result = new MethodFragment()
			{
				Name = Name,
				ReturnType = ReturnType.Resolve(Context.TypeMapper)
			};

			foreach (var parameter in Parameters)
			{
				result.Parameters.Add(parameter.Build());
			}

			foreach (var hint in Hints)
			{
				result.Hints.Add(hint);
			}

			return result;
		}
	}
}
