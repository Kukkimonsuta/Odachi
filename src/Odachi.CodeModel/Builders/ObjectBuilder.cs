using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Description;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class ObjectBuilder : TypeFragmentBuilderBase<ObjectBuilder, ObjectFragment>
	{
		public ObjectBuilder(PackageContext context, string name, IReadOnlyList<string> genericArguments, object source)
			: base(context, name)
		{
			GenericArguments = genericArguments ?? Array.Empty<string>();
			Source = source;

			context.ObjectDescriptors.Describe(this);
		}

		public IReadOnlyList<string> GenericArguments { get; }
		public IList<ConstantBuilder> Constants { get; } = new List<ConstantBuilder>();
		public IList<FieldBuilder> Fields { get; } = new List<FieldBuilder>();
		public object Source { get; }

		public ObjectBuilder Constant(string name, ITypeReference type, object value, Action<ConstantBuilder> configure = null)
		{
			return Constant(name, type, null, configure: configure);
		}
		public ObjectBuilder Constant(string name, ITypeReference type, object value, object source, Action<ConstantBuilder> configure = null)
		{
			var constantBuilder = new ConstantBuilder(Context, name, type, value, source);

			configure?.Invoke(constantBuilder);

			Constants.Add(constantBuilder);

			return this;
		}

		public ObjectBuilder Field(string name, ITypeReference type, Action<FieldBuilder> configure = null)
		{
			return Field(name, type, null, configure: configure);
		}
		public ObjectBuilder Field(string name, ITypeReference type, object source, Action<FieldBuilder> configure = null)
		{
			var fieldBuilder = new FieldBuilder(Context, name, type, source);

			configure?.Invoke(fieldBuilder);

			Fields.Add(fieldBuilder);

			return this;
		}

		public override ObjectFragment Build()
		{
			var result = new ObjectFragment()
			{
				Name = Name,
				ModuleName = ModuleName,
				GenericArguments = GenericArguments.Select(a => new GenericArgumentDefinition(a)).ToArray(),
			};

			foreach (var constant in Constants)
			{
				result.Constants.Add(constant.Build());
			}

			foreach (var field in Fields)
			{
				result.Fields.Add(field.Build());
			}

			foreach (var hint in Hints)
			{
				result.Hints.Add(hint);
			}

			return result;
		}
	}
}
