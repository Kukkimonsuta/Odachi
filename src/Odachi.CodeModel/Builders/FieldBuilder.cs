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
		public FieldBuilder(PackageContext context, string name, Type type, object source)
			: base(context, name)
		{
			Type = new TypeReferenceBuilder(context, type ?? throw new ArgumentNullException(nameof(type)), source);
			Source = source;

			Context.FieldDescriptors.Describe(this);
		}

		public TypeReferenceBuilder Type { get; }
		public object Source { get; }

		public override FieldFragment Build()
		{
			var result = new FieldFragment()
			{
				Name = Name,
				Type = Type.Build(),
			};

			foreach (var hint in Hints)
			{
				result.Hints.Add(hint);
			}

			return result;
		}
	}
}
