using Odachi.CodeModel.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class EnumItemBuilder : BuilderBase<EnumItemBuilder, EnumItemFragment>
	{
		public EnumItemBuilder(PackageContext context, string name, int value, object source)
			: base(context, name)
		{

			Source = source;

			Value = value;
			context.EnumItemDescriptors.Describe(this);
		}

		public int Value { get; }
		public object Source { get; }

		public override EnumItemFragment Build()
		{
			var result = new EnumItemFragment()
			{
				Name = Name,
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
