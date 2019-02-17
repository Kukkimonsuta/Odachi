using Odachi.CodeModel.Description;
using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class MethodBuilder : FragmentBuilderBase<MethodBuilder, MethodFragment>
	{
		public MethodBuilder(PackageContext context, string name, Type returnType, object source)
			: base(context, name)
		{
			ReturnType = new TypeReferenceBuilder(context, returnType ?? throw new ArgumentNullException(nameof(returnType)));
			Source = source;

			Context.MethodDescriptors.Describe(this);
		}

		public IList<ParameterBuilder> Parameters { get; } = new List<ParameterBuilder>();
		public TypeReferenceBuilder ReturnType { get; }
		public object Source { get; }

		public MethodBuilder Parameter(string name, Type type, object source, Action<ParameterBuilder> configure = null)
		{
			var parameterBuilder = new ParameterBuilder(Context, name, type, source);

			configure?.Invoke(parameterBuilder);

			Parameters.Add(parameterBuilder);

			return this;
		}

		public override MethodFragment Build()
		{
			var result = new MethodFragment()
			{
				Name = Name,
				ReturnType = ReturnType.Build(),
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
