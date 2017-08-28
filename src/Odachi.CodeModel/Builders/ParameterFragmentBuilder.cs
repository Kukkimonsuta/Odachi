using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class ParameterFragmentBuilder : BuilderBase<ParameterFragmentBuilder, ParameterFragment>
	{
		public ParameterFragmentBuilder(PackageContext context, string name, ITypeReference type, object source)
			: base(context, name)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			Type = type;
			Source = source;

			// todo: describe
		}

		public ITypeReference Type { get; }
		public object Source { get; }

		public override ParameterFragment Build()
		{
			var result = new ParameterFragment()
			{
				Name = Name,
				Type = Type.Resolve(Context.TypeMapper),
			};

			return result;
		}
	}
}
