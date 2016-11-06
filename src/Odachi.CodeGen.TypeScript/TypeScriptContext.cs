using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Odachi.CodeGen.TypeScript
{
	[Flags]
	public enum TypeScriptModuleFlags
	{
		BuiltIn = 0x0001,
		External = 0x0002,
		Generic = 0x0004,
		Primitive = 0x0008,
		Anonymous = 0x0010,
	}

	public struct TypeScriptModule
	{
		public TypeScriptModuleFlags Flags { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }

		public Type ManagedType { get; set; }
	}

	public class TypeScriptContext : IEnumerable<TypeScriptModule>
	{
		private IReadOnlyCollection<TypeScriptModule> DefaultModules = new[]
		{
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(bool), Name = "boolean" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(sbyte), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(byte), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(short), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(ushort), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(int), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(uint), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(long), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(ulong), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(float), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(double), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(decimal), Name = "number" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.Primitive, ManagedType = typeof(string), Name = "string" },
			new TypeScriptModule() { Flags = TypeScriptModuleFlags.BuiltIn, ManagedType = typeof(DateTime), Name = "Date" },
		};

		public TypeScriptContext(ITypeScriptTemplateResolver templateResolver)
		{
			_templateResolver = templateResolver;
			_modules = DefaultModules.ToDictionary(t => t.ManagedType);
		}

		private ITypeScriptTemplateResolver _templateResolver;
		private IDictionary<Type, TypeScriptModule> _modules;

		public bool TryGetModule(Type managedType, out TypeScriptModule module)
		{
			managedType = Nullable.GetUnderlyingType(managedType) ?? managedType;

			if (_modules.TryGetValue(managedType, out module))
			{
				return true;
			}

			if (managedType.GetTypeInfo().IsGenericType && _modules.TryGetValue(managedType.GetGenericTypeDefinition(), out module))
			{
				return true;
			}

			module = default(TypeScriptModule);
			return false;
		}

		public TypeScriptModule GetModule(Type managedType)
		{
			TypeScriptModule result;

			if (TryGetModule(managedType, out result))
				return result;

			throw new InvalidOperationException($"Undefined mapping for managed type '{managedType.FullName ?? managedType.Name}'");
		}

		public void AddBuiltin<T>(string name, TypeScriptModuleFlags flags = 0, bool overwrite = false)
		{
			AddBuiltin(typeof(T), name, flags: flags, overwrite: overwrite);
		}
		public void AddBuiltin(Type type, string name, TypeScriptModuleFlags flags = 0, bool overwrite = false)
		{
			if (!overwrite && _modules.ContainsKey(type))
				throw new InvalidOperationException($"Context already contains builtin for '{type.FullName}'");

			_modules[type] = new TypeScriptModule()
			{
				Flags = TypeScriptModuleFlags.BuiltIn | flags,
				ManagedType = type,
				Name = name,
			};
		}

		public void AddModule<T>(bool overwrite = false)
		{
			AddModule(typeof(T), overwrite: overwrite);
		}
		public void AddModule(Type type, bool overwrite = false)
		{
			if (!overwrite && _modules.ContainsKey(type))
				throw new InvalidOperationException($"Context already contains module for '{type.FullName}'");

			_modules[type] = new TypeScriptModule()
			{
				Flags = (type.GetTypeInfo().IsGenericType ? TypeScriptModuleFlags.Generic : 0) | (type.GetTypeInfo().IsEnum ? TypeScriptModuleFlags.Primitive : 0),
				ManagedType = type,
				Name = TypeScriptHelpers.GetModuleName(type),
				Path = TypeScriptHelpers.GetModulePath(type),
			};
		}

		public void AddModule(TypeScriptModule module, bool overwrite = false)
		{
			if (!overwrite && _modules.ContainsKey(module.ManagedType))
				throw new InvalidOperationException($"Context already contains module for '{module.ManagedType.FullName}'");

			_modules[module.ManagedType] = module;
		}

		public void Save(string rootFolder)
		{
			if (!Directory.Exists(rootFolder))
				Directory.CreateDirectory(rootFolder);

			foreach (var module in _modules.Values.Where(t => (t.Flags & (TypeScriptModuleFlags.BuiltIn | TypeScriptModuleFlags.External)) == 0))
			{
				var fullPath = Path.Combine(rootFolder, module.Path + ".ts");

				var template = _templateResolver.GetTemplate(module);

				using (var writer = new TypeScriptWriter(this))
				{
					template.Write(writer, module);

					using (var output = new StreamWriter(File.Create(fullPath)))
						writer.Save(output);
				}
			}
		}

		#region IEnumerable

		public IEnumerator<TypeScriptModule> GetEnumerator()
		{
			return _modules.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _modules.Values.GetEnumerator();
		}

		#endregion
	}
}
