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
			var willRenderImports = _imports.Count > 0;
			var willRenderHelpers = _helpers.Count > 0;

			if (!willRenderImports && !willRenderHelpers)
				return false;

			if (willRenderImports)
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

				if (willRenderHelpers)
				{
					writer.WriteLine();
				}
			}

			if (willRenderHelpers)
			{
				foreach (var helper in _helpers)
				{
					writer.WriteIndented(helper);
				}
			}

			return true;
		}

		public override bool RenderBody(IndentedTextWriter writer, string body)
		{
			if (body.Length <= 0)
			{
				return false;
			}

			writer.WriteIndented(body);

			return true;
		}

		public override bool RenderFooter(IndentedTextWriter writer)
		{
			var didRender = false;

			if (_defaultExport != null)
			{
				writer.WriteIndented($"export default {_defaultExport};");

				didRender = true;
			}
			if (_exports.Count > 0)
			{
				writer.WriteIndented($"export {{ {string.Join(", ", _exports)} }};");

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
		/// Return string representation valid in code of a type reference.
		/// </summary>
		public string Resolve(TypeReference type, bool includeNullability = true)
		{
			var nullableSuffix = includeNullability && type.IsNullable ? " | null" : "";

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

						return $"core.Page<{Resolve(type.GenericArguments[0])}>{nullableSuffix}";

					case "OneOf":
						if (type.GenericArguments?.Length < 2 || type.GenericArguments?.Length > 9)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

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

			return $"{type.Name}{(type.GenericArguments?.Length > 0 ? $"<{string.Join(", ", type.GenericArguments.Select(a => Resolve(a)))}>" : "")}{nullableSuffix}";
		}

		/// <summary>
		/// Due to inability of JS deserializers to use classes we first deserialize raw transport into js objects and then map them to generated classes.
		/// </summary>
		public string CreateExpression(TypeReference type, string source)
		{
			var prefix = $"{source} === undefined || {source} === null ? ";
			if (type.IsNullable)
			{
				prefix += "null : ";
			}
			else
			{
				Helper("function fail(message: string): never { throw new Error(message); }");

				prefix += $"fail('Contract violation: value of \\'{source}\\' cannot be null') : ";
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

					case "boolean":
					case "string":
					case "integer":
					case "long":
					case "float":
					case "double":
					case "decimal":
						return $"({prefix}{source})";

					case "datetime":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("moment", "* as moment");
						return $"({prefix}moment({source}))";

					case "array":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						var itemName = $"{source.Replace(".", "$")}$_item_";

						return $"({prefix}{source}.map(({itemName}: any) => {CreateExpression(type.GenericArguments[0], itemName)}))";

					case "PagingOptions":
						return $"({prefix}{source} as {Resolve(type)})";

					case "Page":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						Import("@stackino/uno", "core");

						return $"({prefix}new core.Page<{Resolve(type.GenericArguments[0])}>({CreateExpression(new TypeReference(null, "array", TypeKind.Array, false, type.GenericArguments[0]), $"{source}.data")}, {CreateExpression(new TypeReference(null, "integer", TypeKind.Primitive, false), $"{source}.number")}, {CreateExpression(new TypeReference(null, "integer", TypeKind.Primitive, false), $"{source}.count")}))";

					case "OneOf":
						if (type.GenericArguments?.Length < 2 || type.GenericArguments?.Length > 9)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						string makeTernary(string condition, string yes, string no)
						{
							return $"{condition} ? ({yes}) : ({no})";
						}

						string checkIndex(int index)
						{
							if (type.GenericArguments.Length <= index)
								return $"fail(`Contract violation: cannot handle OneOf index ${{{source}.index}} of \\'{source}\\'`)";

							var t = type.GenericArguments[index];

							return makeTernary($"{source}.index === {index + 1}", CreateExpression(type.GenericArguments[index], $"{source}.option{index + 1}"), checkIndex(index + 1));
						}

						return $"{prefix}{checkIndex(0)}";

					case "ValidationState":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("@stackino/uno", "validation");

						return $"{prefix}new validation.ValidationState({source}.state)";

					default:
						throw new NotSupportedException($"Undefined behavior for builtin '{type.Name}'");
				}
			}

			Import(type);

			return $"{prefix}{Resolve(type, includeNullability: false)}.create({source})";
		}

		#endregion
	}
}
