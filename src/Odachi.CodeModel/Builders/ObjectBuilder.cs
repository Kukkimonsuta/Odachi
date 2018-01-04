using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Description;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class ObjectBuilder : BuilderBase<ObjectBuilder, ObjectFragment>, ITypeFragmentBuilder
	{
		public ObjectBuilder(PackageContext context, string name, object source)
			: base(context, name)
		{
			Source = source;

			context.ObjectDescriptors.Describe(this);
		}

		public IList<FieldBuilder> Fields { get; } = new List<FieldBuilder>();
		public object Source { get; }

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
				Name = Name
			};

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

		#region ITypeFragmentBuilder

		TypeFragment ITypeFragmentBuilder.Build()
		{
			return Build();
		}

		#endregion
	}
}
