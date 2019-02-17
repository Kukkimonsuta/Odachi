using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Builders.Abstraction
{
	public abstract class BuilderBase<TSelf, TItem>
		where TSelf : BuilderBase<TSelf, TItem>
	{
		public BuilderBase(PackageContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public PackageContext Context { get; }

		public IDictionary<string, string> Hints { get; } = new Dictionary<string, string>();

		public TSelf Hint(string key, string value)
		{
			Hints[key] = value;

			return (TSelf)this;
		}

		public abstract TItem Build();
	}

	public abstract class FragmentBuilderBase<TSelf, TItem> : BuilderBase<TSelf, TItem>
		where TSelf : FragmentBuilderBase<TSelf, TItem>
	{
		public FragmentBuilderBase(PackageContext context, string name)
			: base(context)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}

		public string Name { get; }
	}

	public abstract class TypeFragmentBuilderBase<TSelf, TItem> : FragmentBuilderBase<TSelf, TItem>
		where TSelf : FragmentBuilderBase<TSelf, TItem>
	{
		public TypeFragmentBuilderBase(PackageContext context, string name)
			: base(context, name)
		{
			ModuleName = context.MapPath(context.GlobalDescriptor.GetModuleName(context, name));
		}

		public string ModuleName { get; }
	}
}
