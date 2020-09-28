using System;

namespace Odachi.CodeModel.Builders.Abstraction
{
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
}
