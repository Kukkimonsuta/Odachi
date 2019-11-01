using Odachi.CodeModel.Description;
using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class ConstantBuilder : FragmentBuilderBase<ConstantBuilder, ConstantFragment>
	{
		public ConstantBuilder(PackageContext context, string name, Type type, object value, object source)
			: base(context, name)
		{
			Type = new TypeReferenceBuilder(context, type ?? throw new ArgumentNullException(nameof(type)), source);
			Value = value;
			Source = source;

			Context.ConstantDescriptors.Describe(this);
		}

		public TypeReferenceBuilder Type { get; }
		public object Value { get; }
		public object Source { get; }

		public override ConstantFragment Build()
		{
			var result = new ConstantFragment()
			{
				Name = Name,
				Type = Type.Build(),
				Value = Value,
			};

			foreach (var hint in Hints)
			{
				result.Hints.Add(hint);
			}

			return result;
		}
	}
}
