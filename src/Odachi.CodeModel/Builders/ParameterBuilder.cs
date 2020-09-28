using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;
using Odachi.CodeModel.Description;

namespace Odachi.CodeModel.Builders
{
	public class ParameterBuilder : FragmentBuilderBase<ParameterBuilder, ParameterFragment>
	{
		public ParameterBuilder(PackageContext context, string name, Type type, object source)
			: base(context, name)
		{
			Type = new TypeReferenceBuilder(context, type ?? throw new ArgumentNullException(nameof(type)), source);
			Source = source;

			Context.ParameterDescriptors.Describe(this);
		}

		public TypeReferenceBuilder Type { get; }
		public object Source { get; }

		public override ParameterFragment Build()
		{
			var result = new ParameterFragment()
			{
				Name = Name,
				Type = Type.Build(),
			};

			return result;
		}
	}
}
