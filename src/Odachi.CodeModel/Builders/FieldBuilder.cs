using Odachi.CodeModel.Description;
using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class FieldBuilder : FragmentBuilderBase<FieldBuilder, FieldFragment>
	{
		public FieldBuilder(PackageContext context, string name, ITypeReference type, object source)
			: base(context, name)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			Type = type;
			Source = source;

			Context.FieldDescriptors.Describe(this);
		}

		public ITypeReference Type { get; }
		public object Source { get; }

		public override FieldFragment Build()
		{
			var result = new FieldFragment()
			{
				Name = Name,
				Type = Type.Resolve(Context.TypeMapper),
			};

			foreach (var hint in Hints)
			{
				result.Hints.Add(hint);
			}

			return result;
		}
	}
}
