using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptWriter : IDisposable
	{
		public TypeScriptWriter(TypeScriptContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			Context = context;

			_writer = new StringWriter();
			_imports = new Dictionary<string, HashSet<string>>();
		}

		private Dictionary<string, HashSet<string>> _imports;
		private StringWriter _writer;
		private int _indent;

		public TypeScriptContext Context { get; protected set; }

		public string IndentString { get; set; } = "\t";

		public void Indent(int magnitude)
		{
			_indent = Math.Max(0, _indent + magnitude);
		}

		public void WriteIndent()
		{
			for (var i = 0; i < _indent; i++)
				_writer.Write(IndentString);
		}

		public void Write(string text) => _writer.Write(text);
		public void Write(string format, params object[] args) => Write(string.Format(format, args));

		public void WriteLine() => _writer.WriteLine();
		public void WriteLine(string text) => _writer.WriteLine(text);
		public void WriteLine(string format, params object[] args) => WriteLine(string.Format(format, args));

		public void WriteIndented(string format, params object[] args)
		{
			WriteIndented(string.Format(format, args));
		}
		public void WriteIndented(string text)
		{
			foreach (var line in text.Replace("\r\n", "\n").Split(new[] { '\n' }, StringSplitOptions.None))
			{
				if (string.IsNullOrWhiteSpace(line))
				{
					WriteLine();
					continue;
				}

				WriteIndent();

				WriteLine(line);
			}
		}

		public string GetFqTsType(Type type, bool registerImport = true)
		{
			return GetFqTsTypeInternal(type, registerImport, type.Name);
		}
		private string GetFqTsTypeInternal(Type type, bool registerImport, string debugStack)
		{
			// handle generic parameters
			if (type.IsGenericParameter)
			{
				return type.Name;
			}

			// try to resolve from registered modules/builtins
			TypeScriptModule module;
			if (!Context.TryGetModule(type, out module))
			{
				var interfaces = type.GetTypeInfo().GetInterfaces();

				if (type.GetTypeInfo().IsInterface)
				{
					interfaces = interfaces
						.Concat(new[] { type })
						.ToArray();
				}

				// try resolve enumerables by making them an array
				var enumerableInterface = interfaces.FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
				if (enumerableInterface != null)
				{
					var itemType = enumerableInterface.GetTypeInfo().GetGenericArguments().Single();
					var moduleName = GetFqTsTypeInternal(itemType, registerImport, $"{debugStack} => {itemType.Name}");

					return moduleName + "[]";
				}

				throw new InvalidOperationException($"Undefined mapping for managed type '{debugStack}'");
			}

			var genericSuffix = "";

			if (module.Flags.HasFlag(TypeScriptModuleFlags.Generic) && type.GetTypeInfo().IsGenericType)
			{
				var arguments = type.GetTypeInfo().GetGenericArguments();
				var argumentFqTypes = arguments.Select(a => GetFqTsTypeInternal(a, registerImport, $"{debugStack} => {a.Name}"));

				genericSuffix = $"<{string.Join(",", argumentFqTypes)}>";
			}

			if (registerImport && !module.Flags.HasFlag(TypeScriptModuleFlags.BuiltIn))
			{
				AddImport(module.Name, module.Path);
			}

			return module.Name + genericSuffix;
		}

		public class FqTsType2
		{
			public FqTsType2(string name, TypeScriptModule? module, Type managedType, IEnumerable<FqTsType2> genericArguments = null, int arrayDimensions = 0)
			{
				Name = name;
				Module = module;
				ManagedType = managedType;
				GenericArguments = genericArguments ?? Enumerable.Empty<FqTsType2>();
				ArrayDimensions = arrayDimensions;
			}

			/// <summary>
			/// Managed type without array modifier
			/// </summary>
			public Type ManagedType { get; }

			public TypeScriptModule? Module { get; }

			public string Name { get; }

			public IEnumerable<FqTsType2> GenericArguments { get; }

			public int ArrayDimensions { get; }

			public bool IsArray => ArrayDimensions > 0;

			public bool IsGeneric => GenericArguments.Any();

			public override string ToString()
			{
				var genericArguments = ToGenericArgumentsString();
				var array = ToArrayString();

				return $"{Name}{genericArguments}{array}";
			}

			public string ToGenericArgumentsString()
			{
				return IsGeneric ? $"<{string.Join(", ", GenericArguments.Select(a => a.ToString()))}>" : "";
			}

			public string ToArrayString()
			{
				return IsArray ? string.Concat(Enumerable.Repeat("[]", ArrayDimensions)) : "";
			}
		}

		public FqTsType2 GetFqTsType2(Type type, bool registerImport = true)
		{
			return GetFqTsTypeInternal2(type, registerImport, type.Name);
		}
		private FqTsType2 GetFqTsTypeInternal2(Type type, bool registerImport, string debugStack, int arrayDimensions = 0)
		{
			// handle generic parameters
			if (type.IsGenericParameter)
			{
				if (arrayDimensions > 0)
				{
					throw new InvalidOperationException($"Generic parameter cannot be an array '{debugStack}'");
				}

				return new FqTsType2(type.Name, null, type);
			}

			// try to resolve from registered modules/builtins
			TypeScriptModule module;
			if (!Context.TryGetModule(type, out module))
			{
				// obtain all relevant interfaces
				var interfaces = type.GetTypeInfo().GetInterfaces();

				if (type.GetTypeInfo().IsInterface)
				{
					// given type is interface, include it also
					interfaces = interfaces
						.Concat(new[] { type })
						.ToArray();
				}

				// try resolve enumerables by making them an array
				var enumerableInterface = interfaces.FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
				if (enumerableInterface != null)
				{
					var itemType = enumerableInterface.GetTypeInfo().GetGenericArguments().Single();

					// resolve enumerable item type
					return GetFqTsTypeInternal2(itemType, registerImport, $"{debugStack} => {itemType.Name}", arrayDimensions: arrayDimensions + 1);
				}

				throw new InvalidOperationException($"Undefined mapping for managed type '{debugStack}'");
			}

			IEnumerable<FqTsType2> genericArguments = null;
			if (module.Flags.HasFlag(TypeScriptModuleFlags.Generic) && type.GetTypeInfo().IsGenericType)
			{
				genericArguments = type.GetTypeInfo().GetGenericArguments()
					.Select(a => GetFqTsTypeInternal2(a, registerImport, $"{debugStack} => {a.Name}"))
					.ToArray();
			}

			if (registerImport && !module.Flags.HasFlag(TypeScriptModuleFlags.BuiltIn))
			{
				AddImport(module.Name, module.Path);
			}

			return new FqTsType2(module.Name, module, type, genericArguments: genericArguments, arrayDimensions: arrayDimensions);
		}

		public TypeScriptBlock WriteBlock(string prefix = null, string suffix = null)
		{
			return new TypeScriptBlock(this, prefix: prefix, suffix: suffix);
		}

		public TypeScriptEnumBlock WriteEnum(string name)
		{
			return new TypeScriptEnumBlock(this, name);
		}

		public TypeScriptInterfaceBlock WriteInterface(string name, string[] extends = null)
		{
			return new TypeScriptInterfaceBlock(this, name, extends);
		}

		public TypeScriptClassBlock WriteClass(string name, string[] extends = null, string[] implements = null, string[] decorators = null)
		{
			return new TypeScriptClassBlock(this, name, extends, implements, decorators);
		}

		public void AddImport(string name, string path)
		{
			HashSet<string> names;
			if (!_imports.TryGetValue(path, out names))
			{
				_imports[path] = names = new HashSet<string>();
			}

			names.Add(name);
		}

		public void WriteEnumMember(string name, long value)
		{
			WriteIndented($"{name} = {value},");
		}
		public void WriteEnumMember<T>(T value)
		{
			var type = typeof(T);

			if (!type.GetTypeInfo().IsEnum)
				throw new ArgumentException("Parameter must be an enum", nameof(value));

			WriteEnumMember(Enum.GetName(typeof(T), value), (long)Convert.ChangeType(value, typeof(long)));
		}

		public void WriteInterfaceMember(string name, string fqTsType)
		{
			WriteIndented($"{name}: {fqTsType},");
		}
		public void WriteInterfaceMember(PropertyInfo property)
		{
			var fqTsType = GetFqTsType(property.PropertyType);

			WriteInterfaceMember(TypeScriptHelpers.GetPropertyName(property.Name), fqTsType);
		}

		public void WriteClassMember(string name, string fqTsType, string value = null, string[] decorators = null)
		{
			WriteIndent();

			if (decorators != null && decorators.Any())
			{
				foreach (var decorator in decorators)
				{
					Write($"@{decorator} ");
				}
			}

			Write($"{name}: {fqTsType};");

			if (value != null)
			{
				Write($" = {value};");
			}

			WriteLine();
		}
		public void WriteClassMember(PropertyInfo property, string value = null, string[] decorators = null)
		{
			var fqTsType = GetFqTsType(property.PropertyType);

			WriteClassMember(TypeScriptHelpers.GetPropertyName(property.Name), fqTsType, value: value, decorators: decorators);
		}

		public void WriteObjectMember(string name, string fqTsType, string value)
		{
			WriteIndent();
			Write($"{name}: {value} as {fqTsType};");
			WriteLine();
		}
		public void WriteObjectMember(PropertyInfo property, string value)
		{
			var fqTsType = GetFqTsType(property.PropertyType);

			WriteClassMember(TypeScriptHelpers.GetPropertyName(property.Name), fqTsType, value);
		}

		public void WriteExports(string @default, string[] named = null)
		{
			var computedNamed = new[] { @default }
				.Concat(named.AsEnumerable() ?? Enumerable.Empty<string>())
				.Distinct();

			WriteIndented($"export default {@default};");
			WriteIndented($"export {{ {string.Join(", ", computedNamed)} }};");
		}

		public void WriteCommentBlock(string text)
		{
			WriteIndent();
			WriteLine("/*");
			foreach (var line in text.Replace("\r\n", "\n").Split(new[] { '\n' }, StringSplitOptions.None))
			{
				if (string.IsNullOrWhiteSpace(line))
				{
					WriteLine();
					continue;
				}

				WriteIndent();
				WriteLine($" * {line}");
			}
			WriteIndent();
			WriteLine(" */");
		}

		public void Save(TextWriter writer)
		{
			if (_imports.Count > 0)
			{
				foreach (var import in _imports)
				{
					writer.WriteLine($"import {{ {string.Join(", ", import.Value.Where(v => !v.Contains("*")))} }} from '{import.Key}';");

					foreach (var value in import.Value.Where(v => v.Contains("*")))
					{
						writer.WriteLine($"import {value} from '{import.Key}';");
					}
				}
				writer.WriteLine();
			}

			writer.Write(
				_writer.GetStringBuilder().ToString()
			);
		}

		public void Dispose()
		{
			if (_writer != null)
			{
				_writer.Dispose();
				_writer = null;
			}
		}
	}
}
