using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Description;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class ServiceBuilder : BuilderBase<ServiceBuilder, ServiceFragment>, ITypeFragmentBuilder
	{
		public ServiceBuilder(PackageContext context, string name, object source)
			: base(context, name)
		{
			Source = source;

			context.ServiceDescriptors.Describe(this);
		}
		
		public IList<MethodBuilder> Methods { get; } = new List<MethodBuilder>();
		public object Source { get; }

		public ServiceBuilder Method(string name, ITypeReference returnType, Action<MethodBuilder> configure = null)
		{
			return Method(name, returnType, configure: configure);
		}
		public ServiceBuilder Method(string name, ITypeReference returnType, object source, Action<MethodBuilder> configure = null)
		{
			var methodBuilder = new MethodBuilder(Context, name, returnType, source);

			configure?.Invoke(methodBuilder);

			Methods.Add(methodBuilder);

			return this;
		}

		public override ServiceFragment Build()
		{
			var result = new ServiceFragment()
			{
				Name = Name
			};
			
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
