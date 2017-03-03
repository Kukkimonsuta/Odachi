using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Odachi.CodeGen.CSharp
{
	public class CSharpContext
	{
		public CSharpContext()
		{
			Mapping.Add(new CSharpMapping(typeof(bool)));
			Mapping.Add(new CSharpMapping(typeof(sbyte)));
			Mapping.Add(new CSharpMapping(typeof(byte)));
			Mapping.Add(new CSharpMapping(typeof(short)));
			Mapping.Add(new CSharpMapping(typeof(ushort)));
			Mapping.Add(new CSharpMapping(typeof(int)));
			Mapping.Add(new CSharpMapping(typeof(uint)));
			Mapping.Add(new CSharpMapping(typeof(float)));
			Mapping.Add(new CSharpMapping(typeof(double)));
			Mapping.Add(new CSharpMapping(typeof(decimal)));
			Mapping.Add(new CSharpMapping(typeof(string)));
			Mapping.Add(new CSharpMapping(typeof(DateTime)));
			Mapping.Add(new CSharpMapping(typeof(Nullable<>)));
			Mapping.Add(new CSharpMapping(typeof(IEnumerable<>)));
			Mapping.Add(new CSharpMapping(typeof(ICollection<>)));
		}

		public IList<CSharpMapping> Mapping { get; } = new List<CSharpMapping>();

		public IList<CSharpPackage> Packages { get; } = new List<CSharpPackage>();

		public CSharpReference GetReference(Type type)
		{
			CSharpMapping mapping;

			if (type.GetTypeInfo().IsGenericType)
			{
				var typeDefinition = type.GetGenericTypeDefinition();

				mapping = Mapping
					.Where(m => m.Type == typeDefinition)
					.FirstOrDefault();
			}
			else
			{
				mapping = Mapping
					.Where(m => m.Type == type)
					.FirstOrDefault();
			}

			if (mapping == null)
				throw new InvalidOperationException($"Cannot resolve reference for type '{type.Name}'");

			var genericArguments = new List<CSharpReference>();
			if (mapping.IsGeneric)
			{
				if (!type.GetTypeInfo().IsGenericType)
					throw new InvalidOperationException($"Cannot map non generic type '{type.Name}' to generic mapping '{mapping.Name}'");

				foreach (var argument in type.GetTypeInfo().IsGenericTypeDefinition ? type.GetTypeInfo().GenericTypeParameters : type.GetTypeInfo().GenericTypeArguments)
				{
					var reference = GetReference(argument);

					genericArguments.Add(reference);
				}
			}

			return new CSharpReference(mapping, genericArguments);
		}

		public void Save(string path)
		{
			foreach (var package in Packages)
			{
				foreach (var module in package.Modules)
				{
					foreach (var fragment in module.Fragments)
					{
						foreach (var mapping in fragment.CreateMappings(this, package, module))
						{
							if (Mapping.Any(m => m.Type == mapping.Type))
							{
								throw new InvalidOperationException($"Mapping for type '{mapping.Type}' already exists");
							}

							Mapping.Add(mapping);
						}
					}
				}
			}

			foreach (var package in Packages)
			{
				var packagePath = Path.Combine(path, package.Name);

				package.Save(this, packagePath);
			}
		}
	}
}
