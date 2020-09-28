namespace Odachi.CodeModel.Builders.Abstraction
{
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
