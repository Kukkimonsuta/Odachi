using Odachi.CodeModel.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class EnumBuilder : TypeFragmentBuilderBase<EnumBuilder, EnumFragment>
	{
		public EnumBuilder(PackageContext context, string name, object source)
			: base(context, name)
		{
			Source = source;

			context.EnumDescriptors.Describe(this);
		}

		public IList<EnumItemBuilder> Items { get; } = new List<EnumItemBuilder>();
		public object Source { get; }

		public EnumBuilder Item(string name, int value, Action<EnumItemBuilder> configure = null)
		{
			var enumItemBuilder = new EnumItemBuilder(Context, name, value, Source);

			configure?.Invoke(enumItemBuilder);

			Items.Add(enumItemBuilder);

			return this;
		}

		public override EnumFragment Build()
		{
			var result = new EnumFragment()
			{
				Name = Name,
				ModuleName = ModuleName,
			};

			foreach (var item in Items)
			{
				result.Items.Add(item.Build());
			}

			foreach (var hint in Hints)
			{
				result.Hints.Add(hint);
			}

			return result;
		}
	}
}
