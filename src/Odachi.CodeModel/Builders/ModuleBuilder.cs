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

		public ModuleBuilder Object(string name, Action<ObjectBuilder> configure)
		{
			return Object(name, null, configure);
		}
		public ModuleBuilder Object(string name, Type type, Action<ObjectBuilder> configure)
		{
			var objectBuilder = new ObjectBuilder(Context, name, source: type);

			configure(objectBuilder);

			Fragments.Add(objectBuilder);

			return this;
		}

		public ModuleBuilder Service(string name, Action<ServiceBuilder> configure)
		{
			return Service(name, null, configure);
		}
		public ModuleBuilder Service(string name, Type type, Action<ServiceBuilder> configure)
		{
			var serviceBuilder = new ServiceBuilder(Context, name, source: type);

			configure(serviceBuilder);

			Fragments.Add(serviceBuilder);

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
