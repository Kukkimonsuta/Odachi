using System;
using System.Collections.Generic;
using System.Linq;
using Odachi.CodeGen.TypeScript.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;

namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptModuleContext : ModuleContext
	{
		public TypeScriptModuleContext(Package package, Module module)
		{
			if (package == null)
				throw new ArgumentNullException(nameof(package));
			if (module == null)
				throw new ArgumentNullException(nameof(module));

			Package = package;
			Module = module;
			FileName = TS.ModuleFileName(module.Name);
		}

		private Dictionary<string, (List<string> named, string all)> _imports = new Dictionary<string, (List<string>, string)>();
		private List<string> _helpers = new List<string>();
		private List<string> _exports = new List<string>();
		private string _defaultExport = null;

		#region General

		public override bool RenderHeader(IndentedTextWriter writer)
		{
			var didRender = false;

			if (_imports.Count > 0)
			{
				foreach (var group in _imports.OrderBy(i => i.Key))
				{
					var name = group.Key;

					if (name.StartsWith("."))
					{
						name = PathTools.GetRelativePath(Module.Name, group.Key);
					}

					writer.WriteIndent();
					if (group.Value.all != null)
					{
						writer.WriteLine($"import {group.Value.all} from '{name}';");
					}
					if (group.Value.named.Count > 0)
					{
						writer.WriteLine($"import {{ {string.Join(", ", group.Value.named.OrderBy(n => char.IsLower(n[0]) ? 0 : 1).ThenBy(n => n))} }} from '{name}';");
					}
				}

				writer.WriteSeparatingLine();

				didRender = true;
			}

			if (_helpers.Count > 0)
			{
				foreach (var helper in _helpers)
				{
					writer.WriteIndentedLine(helper);
				}

				writer.WriteSeparatingLine();

				didRender = true;
			}

			return didRender;
		}

		public override bool RenderBody(IndentedTextWriter writer, string body)
		{
			if (body.Length <= 0)
			{
				return false;
			}

			writer.WriteIndentedLine(body);
			writer.WriteSeparatingLine();

			return true;
		}

		public override bool RenderFooter(IndentedTextWriter writer)
		{
			var didRender = false;

			if (_defaultExport != null)
			{
				writer.WriteIndentedLine($"export default {_defaultExport};");

				didRender = true;
			}
			if (_exports.Count > 0)
			{
				writer.WriteIndentedLine($"export {{ {string.Join(", ", _exports)} }};");

				didRender = true;
			}

			return didRender;
		}

		#endregion

		#region TS specific

		/// <summary>
		/// Import type reference so that it's `Resolve`d representation is available in current modules scope.
		/// </summary>
		public void Import(TypeReference type)
		{
			if (type.Module != null)
			{
				Import(TS.ModuleName(type.Module), type.Name);
			}

			foreach (var genericArgument in type.GenericArguments)
			{
				Import(genericArgument);
			}
		}
		/// <summary>
		/// Import type from module so that import is available in current modules scope.
		/// </summary>
		/// <param name="module">Module name, for instance `react`.</param>
		/// <param name="import">Import name, for instance `Component` => `import { Component }` or `* as React` to render `import * as React`.</param>
		public void Import(string module, string import)
		{
			if (!_imports.TryGetValue(module, out var imports))
			{
				imports = (new List<string>(), null);
			}

			if (import.StartsWith("*"))
			{
				imports.all = import;
			}
			else
			{
				if (!imports.named.Contains(import))
				{
					imports.named.Add(import);
				}
			}

			_imports[module] = imports;
		}

		/// <summary>
		/// Arbitrary piece of code rendered between imports and body.
		/// </summary>
		public void Helper(string helper)
		{
			if (_helpers.Contains(helper))
				return;

			_helpers.Add(helper);
		}

		/// <summary>
		/// Add type to exported type list.
		/// </summary>
		public void Export(string name, bool @default = false)
		{
			if (!_exports.Contains(name))
				_exports.Add(name);

			if (@default)
			{
				_defaultExport = name;
			}
		}

		/// <summary>
		/// Returns string representation valid in code of a type reference.
		/// </summary>
		public string Resolve(TypeReference type, bool includeNullability = true, bool includeGenericArguments = true)
		{
			var nullableSuffix = includeNullability && type.IsNullable ? " | null" : "";

			if (type.Kind == TypeKind.GenericParameter)
			{
				return $"{type.Name}{nullableSuffix}";
			}

			if (type.Module == null)
			{
				// handle builtins

				switch (type.Name)
				{
					case "void":
					case "boolean":
					case "string":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"{type.Name}{nullableSuffix}";

					case "integer":
					case "long":
					case "float":
					case "double":
					case "decimal":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"number{nullableSuffix}";

					case "datetime":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("moment", "Moment");
						return $"Moment{nullableSuffix}";

					case "array":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						if (!includeGenericArguments)
							return "Array";

						return $"Array<{Resolve(type.GenericArguments[0])}>{nullableSuffix}";

					case "file":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"File{nullableSuffix}";

					case "PagingOptions":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"{{ number: number, size?: number }}{nullableSuffix}";

					case "Page":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("@stackino/uno", "core");

						if (!includeGenericArguments)
							return "core.Page";

						return $"core.Page<{Resolve(type.GenericArguments[0])}>{nullableSuffix}";

					case "Tuple":
						if (type.GenericArguments?.Length < 1 || type.GenericArguments?.Length > 8)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						if (!includeGenericArguments)
							throw new InvalidOperationException("Cannot resolve tuple without generic arguments");

						return $"[{string.Join(", ", type.GenericArguments.Select(t => Resolve(t)))}]{nullableSuffix}";

					case "OneOf":
						if (type.GenericArguments?.Length < 2 || type.GenericArguments?.Length > 9)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						if (!includeGenericArguments)
							throw new InvalidOperationException("Cannot resolve oneof without generic arguments");

						if (includeNullability && type.GenericArguments.Any(a => a.IsNullable))
							nullableSuffix = " | null";

						return $"{string.Join(" | ", type.GenericArguments.Select(t => Resolve(t, includeNullability: false)))}{nullableSuffix}";

					case "ValidationState":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("@stackino/uno", "validation");

						return $"validation.ValidationState{nullableSuffix}";

					default:
						throw new NotSupportedException($"Undefined behavior for builtin '{type.Name}'");
				}
			}

			Import(type);

			return $"{type.Name}{(includeGenericArguments && type.GenericArguments?.Length > 0 ? $"<{string.Join(", ", type.GenericArguments.Select(a => Resolve(a)))}>" : "")}{nullableSuffix}";
		}

		/// <summary>
		/// Due to inability of JS deserializers to use classes we first deserialize raw transport into js objects and then map them to generated classes.
		/// </summary>
		public string CreateExpression(TypeReference type, string source)
		{
			if (type.Kind == TypeKind.GenericParameter)
			{
				// handle generic parameters

				return $"{Factory(type)}.create({source})";
			}

			if (type.Module == null)
			{
				// handle builtins

				switch (type.Name)
				{
					case "void":
						throw new InvalidOperationException("Cannot create void");

					case "file":
						return "null";

					default:
						var factory = Factory(type);
						return $"{factory}.create({source})";
				}
			}

			// handle rest
			Import(type);

			return $"{Factory(type)}.create({source})";
		}

		/// <summary>
		/// Returns reference to a factory for given type.
		/// </summary>
		public string Factory(TypeReference type)
		{
			const string factoryPrefix = "_$$_factory_";

			//if (type.GenericArguments.Any(t => t.Kind == TypeKind.GenericParameter))
			//{
			//	throw new InvalidOperationException("Cannot create factory for open generic type");
			//}

			if (type.Kind == TypeKind.GenericParameter)
			{
				// assume factory exists
				return $"{type.Name}_factory";
			}

			string MakeFactory(string name, string factory, params string[] genericParameterNames)
			{
				var factoryName = $"{factoryPrefix}{name}";

				if (genericParameterNames.Length != type.GenericArguments.Length)
					throw new InvalidOperationException("Generic parameter count mismatch");

				if (genericParameterNames.Any())
				{
					var genericParameters = string.Join(", ", genericParameterNames);
					var arguments = string.Join(", ", genericParameterNames.Select(n => $"{n}_factory: {{ create: (source: any) => {n} }}"));

					Helper($"function {factoryName}<{genericParameters}>({arguments}) {{ return {{ create: {factory} }}; }}");

					if (!type.IsNullable)
					{
						return factoryName;
					}

					var optFactoryName = $"{factoryName}_opt";
					var argumentForward = string.Join(", ", genericParameterNames.Select(n => $"{n}_factory"));

					Helper($"function {optFactoryName}<{genericParameters}>({arguments}) {{ return {{ create: (source: any) => source === undefined || source === null ? null : {factoryName}({argumentForward}).create(source) }}; }}");
					return optFactoryName;
				}
				else
				{
					Helper($"const {factoryName} = {{ create: {factory} }};");

					if (!type.IsNullable)
					{
						return factoryName;
					}

					var optFactoryName = $"{factoryName}_opt";

					Helper($"const {optFactoryName} = {{ create: (source: any): {Resolve(type)} => source === undefined || source === null ? null : {factoryName}.create(source) }};");
					return optFactoryName;
				}
			}

			if (type.Module == null)
			{
				Helper("function fail(message: string): never { throw new Error(message); }");

				switch (type.Name)
				{
					case "boolean":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return MakeFactory(type.Name, $"(source: any): {Resolve(type, includeNullability: false)} => typeof source === 'boolean' ? source : fail(`Contract violation: expected boolean, got \\'{{typeof(source)}}\\'`)");

					case "string":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return MakeFactory(type.Name, $"(source: any): {Resolve(type, includeNullability: false)} => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \\'{{typeof(source)}}\\'`)");

					case "integer":
					case "long":
					case "float":
					case "double":
					case "decimal":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return MakeFactory("number", $"(source: any): {Resolve(type, includeNullability: false)} => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \\'{{typeof(source)}}\\'`)");

					case "datetime":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("moment", "* as moment");
						return MakeFactory(type.Name, $"(source: any): {Resolve(type, includeNullability: false)} => typeof source === 'string' ? moment(source) : fail(`Contract violation: expected datetime string, got \\'{{typeof(source)}}\\'`)");

					case "array":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						var arrayFactory = MakeFactory(
							type.Name,
							$@"(source: any): Array<T> => Array.isArray(source) ? source.map((item: any) => T_factory.create(item)) : fail(`Contract violation: expected array, got \\'{{typeof(source)}}\\'`)",
							"T"
						);

						return $"{arrayFactory}({Factory(type.GenericArguments[0])})";

					case "PagingOptions":
						return MakeFactory(type.Name, $"(source: any): {Resolve(type, includeNullability: false)} => typeof source === 'object' && source !== null ? source : fail(`Contract violation: expected paging options, got \\'{{typeof(source)}}\\'`)");

					case "Page":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						Import("@stackino/uno", "core");

						var pageFactory = MakeFactory(type.Name, $@"(source: any): core.Page<T> =>
		typeof source === 'object' && source !== null ?
			new core.Page<T>(
				{CreateExpression(new TypeReference(null, "array", TypeKind.Array, false, new TypeReference(null, "T", TypeKind.GenericParameter, false)), "source.data")},
				{CreateExpression(new TypeReference(null, "integer", TypeKind.Primitive, false), "source.number")},
				{CreateExpression(new TypeReference(null, "integer", TypeKind.Primitive, false), "source.count")}
			) :
			fail(`Contract violation: expected page, got \\'{{typeof(source)}}\\'`)", "T");

						return $"{pageFactory}({Factory(type.GenericArguments[0])})";

					case "Tuple":
						if (type.GenericArguments?.Length < 1 || type.GenericArguments?.Length > 8)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						var tupleHelperGenericArguments = new string[type.GenericArguments.Length];
						var tupleHelperBody = $"[";
						for (var i = 0; i < type.GenericArguments.Length; i++)
						{
							var genericArgumentName = $"T{i + 1}";

							tupleHelperGenericArguments[i] = genericArgumentName;
							tupleHelperBody += $"{genericArgumentName}_factory.create(source[{i}])";
							if (i != type.GenericArguments.Length - 1)
							{
								tupleHelperBody += ", ";
							}
						}
						tupleHelperBody += "]";

						var tupleFactory = MakeFactory($"tuple{type.GenericArguments.Length}", $"(source: any): [{string.Join(", ", tupleHelperGenericArguments)}] => {tupleHelperBody}", tupleHelperGenericArguments);

						return $"{tupleFactory}({string.Join(", ", type.GenericArguments.Select(a => Factory(a)))})";

					case "OneOf":
						if (type.GenericArguments?.Length < 2 || type.GenericArguments?.Length > 9)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						var oneOfHelperGenericArguments = new string[type.GenericArguments.Length];
						var oneOfHelperBody = "switch (source.index) { ";
						for (var i = 0; i < type.GenericArguments.Length; i++)
						{
							var genericArgumentName = $"T{i + 1}";

							oneOfHelperGenericArguments[i] = genericArgumentName;
							oneOfHelperBody += $"case {i}: return {genericArgumentName}_factory.create(source.option{i + 1}); ";
						}
						oneOfHelperBody += "default: fail(`Contract violation: cannot handle OneOf index ${source.index}`); ";
						oneOfHelperBody += "}";

						var oneOfFactory = MakeFactory($"oneof{type.GenericArguments.Length}", $"(source: any): {string.Join(" | ", oneOfHelperGenericArguments)} => {{ {oneOfHelperBody} }}", oneOfHelperGenericArguments);

						return $"{oneOfFactory}({string.Join(", ", type.GenericArguments.Select(a => Factory(a)))})";

					case "ValidationState":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("@stackino/uno", "validation");

						return MakeFactory(type.Name, $@"(source: any): {Resolve(type, includeNullability: false)} => typeof source === 'object' && source !== null && typeof source.state === 'object' && source.state !== null ? new validation.ValidationState(source.state) : fail(`Contract violation: expected validation state, got \\'{{typeof(source)}}\\'`)");

					default:
						throw new NotSupportedException($"Undefined behavior for builtin '{type.Name}'");
				}
			}

			Import(type);

			var factoryBase = Resolve(type, includeNullability: false, includeGenericArguments: false);

			if (type.GenericArguments.Any())
			{
				return $"{factoryBase}.create({string.Join(", ", type.GenericArguments.Select(a => Factory(a)))})";
			}

			return factoryBase;
		}

		#endregion
	}
}
