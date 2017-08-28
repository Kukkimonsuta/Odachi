using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class ModuleBuilder : BuilderBase<ModuleBuilder, Module>
	{
		public ModuleBuilder(PackageContext context, string name)
			: base(context, name)
		{
		}

		public IList<ITypeFragmentBuilder> Fragments { get; } = new List<ITypeFragmentBuilder>();

		public ModuleBuilder Class(string name, Action<ClassBuilder> configure)
		{
			return Class(name, null, configure);
		}
		public ModuleBuilder Class(string name, Type type, Action<ClassBuilder> configure)
		{
			var classBuilder = new ClassBuilder(Context, name, source: type);

			configure(classBuilder);

			Fragments.Add(classBuilder);

			return this;
		}

		public ModuleBuilder Enum(string name, Action<EnumBuilder> configure)
		{
			return Enum(name, null, configure);
		}
		public ModuleBuilder Enum(string name, Type type, Action<EnumBuilder> configure)
		{
			var enumBuilder = new EnumBuilder(Context, name, source: type);

			configure(enumBuilder);

			Fragments.Add(enumBuilder);

			return this;
		}

		public override Module Build()
		{
			var result = new Module()
			{
				Name = Name,
			};

			foreach (var fragment in Fragments)
			{
				result.Fragments.Add(fragment.Build());
			}

			foreach (var hint in Hints)
			{
				result.Hints.Add(hint);
			}

			return result;
		}
	}
}
