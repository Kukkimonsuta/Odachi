using System;
using System.Collections.Generic;
using System.Linq;
using Odachi.CodeGen.TypeScript.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;
using Odachi.CodeGen.TypeScript.TypeHandlers;
using Odachi.CodeModel.Mapping;

namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptModuleContext : ModuleContext
	{
		public TypeScriptModuleContext(Package package, Module module, IReadOnlyList<ITypeHandler> typeHandlers, TypeScriptOptions options)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Module = module ?? throw new ArgumentNullException(nameof(module));
			TypeHandlers = typeHandlers ?? throw new ArgumentNullException(nameof(typeHandlers));
			Options = options ?? throw new ArgumentNullException(nameof(options));

			FileName = TS.ModuleFileName(module.Name);
		}

		public IReadOnlyList<ITypeHandler> TypeHandlers { get; }
		public TypeScriptOptions Options { get; }

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

			if (_defaultExport != null && Options.AllowDefaultExports)
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
			if (type.Module != null && type.Module != Module.Name)
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
			for (var i = 0; i < TypeHandlers.Count; i++)
			{
				var result = TypeHandlers[i].Resolve(this, type, includeNullability: includeNullability, includeGenericArguments: includeGenericArguments);

				if (result != null)
				{
					return result;
				}
			}

			throw new InvalidOperationException($"Failed to resolve type '{type.ToString()}'");
		}

		/// <summary>
		/// Returns string representation valid in code of a type reference.
		/// </summary>
		public string ResolveDefaultValue(TypeReference type)
		{
			if (type.IsNullable)
			{
				return "null";
			}

			for (var i = 0; i < TypeHandlers.Count; i++)
			{
				var result = TypeHandlers[i].ResolveDefaultValue(this, type);

				if (result != null)
				{
					return result;
				}
			}

			throw new InvalidOperationException($"Failed to resolve type '{type.ToString()}'");
		}

		/// <summary>
		/// Return a javascript expression converting transport representation into runtime representation of given type reference.
		/// </summary>
		public string CreateExpression(TypeReference type, string source)
		{
			if (type.Kind == TypeKind.GenericParameter)
			{
				// handle generic parameters
				var factory = Factory(type);
				if (factory == null)
				{
					return null;
				}

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
						if (factory == null)
						{
							return null;
						}

						return $"{factory}.create({source})";
				}
			}
			else
			{
				// handle modules

				Import(type);

				return $"{Factory(type)}.create({source})";
			}
		}

		/// <summary>
		/// Returns reference to a factory for given type.
		/// </summary>
		public string Factory(TypeReference type)
		{
			for (var i = 0; i < TypeHandlers.Count; i++)
			{
				var result = TypeHandlers[i].Factory(this, type);

				if (result != null)
				{
					return result;
				}
			}

			throw new InvalidOperationException($"Failed to find factory for type '{type.ToString()}'");
		}

		#endregion
	}
}
