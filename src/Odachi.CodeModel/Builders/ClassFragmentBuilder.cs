using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Description;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class ClassBuilder : BuilderBase<ClassBuilder, ClassFragment>, ITypeFragmentBuilder
	{
		public ClassBuilder(PackageContext context, string name, object source)
			: base(context, name)
		{
			Source = source;

			context.ClassDescriptors.Describe(this);
		}

		public IList<FieldBuilder> Fields { get; } = new List<FieldBuilder>();
		public IList<MethodBuilder> Methods { get; } = new List<MethodBuilder>();
		public object Source { get; }

		public ClassBuilder Field(string name, ITypeReference type, Action<FieldBuilder> configure = null)
		{
			return Field(name, type, null, configure: configure);
		}
		public ClassBuilder Field(string name, ITypeReference type, object source, Action<FieldBuilder> configure = null)
		{
			var fieldBuilder = new FieldBuilder(Context, name, type, source);

			configure?.Invoke(fieldBuilder);

			Fields.Add(fieldBuilder);

			return this;
		}

		public ClassBuilder Method(string name, ITypeReference returnType, Action<MethodBuilder> configure = null)
		{
			return Method(name, returnType, configure: configure);
		}
		public ClassBuilder Method(string name, ITypeReference returnType, object source, Action<MethodBuilder> configure = null)
		{
			var methodBuilder = new MethodBuilder(Context, name, returnType, source);

			configure?.Invoke(methodBuilder);

			Methods.Add(methodBuilder);

			return this;
		}

		public override ClassFragment Build()
		{
			var result = new ClassFragment()
			{
				Name = Name
			};

			foreach (var field in Fields)
			{
				result.Fields.Add(field.Build());
			}

			foreach (var method in Methods)
			{
				result.Methods.Add(method.Build());
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
