using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Odachi.CodeGen.CSharp.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;

namespace Odachi.CodeGen.CSharp
{
	public class CSharpModuleContext : ModuleContext<CSharpOptions>
	{
		public CSharpModuleContext(CSharpPackageContext packageContext, string moduleName)
		{
			PackageContext = packageContext ?? throw new ArgumentNullException(nameof(packageContext));
			ModuleName = moduleName ?? throw new ArgumentNullException(nameof(moduleName));
			FileName = CS.ModuleFileName(ModuleName);

			PackageNamespace = packageContext.Options.Namespace ?? throw new ArgumentNullException(nameof(packageContext.Options));
			ModuleNamespace = CS.ModuleNamespace(PackageNamespace, ModuleName);
		}

		public string PackageNamespace { get; }
		public string ModuleNamespace { get; }

		private StringBuilder _bodyBuilder = new StringBuilder();
		private List<string> _imports = new List<string>();

		#region General

		public override bool RenderHeader(IndentedTextWriter writer)
		{
			if (_imports.Count <= 0)
			{
				return false;
			}

			foreach (var @namespace in _imports.OrderBy(x => x))
			{
				writer.WriteLine($"using {@namespace};");
			}
			writer.WriteSeparatingLine();

			return true;
		}

		public override bool RenderBody(IndentedTextWriter writer, string body)
		{
			if (body.Length <= 0)
			{
				return false;
			}

			using (writer.WriteIndentedBlock(prefix: $"namespace {ModuleNamespace} ", writeSeparatingLine: false))
			{
				writer.WriteIndentedLine(body);
			}
			writer.WriteSeparatingLine();

			return true;
		}

		public override bool RenderFooter(IndentedTextWriter writer)
		{
			return false;
		}

		#endregion

		#region CS specific

		public void Import(TypeReference type)
		{
			if (type.Module != null && type.Module != ModuleName)
			{
				var @namespace = CS.ModuleNamespace(PackageNamespace, type.Module);

				Import(@namespace);
			}

			foreach (var genericArgument in type.GenericArguments)
			{
				Import(genericArgument);
			}
		}

		public void Import(string @namespace)
		{
			if (ModuleNamespace.StartsWith(@namespace))
			{
				return;
			}

			if (_imports.Contains(@namespace))
			{
				return;
			}

			_imports.Add(@namespace);
		}

		/// <summary>
		/// Return string representation valid in code of a type reference.
		/// </summary>
		public string Resolve(TypeReference type)
		{
			var includeNullability = PackageContext.Options.EnableNullableReferenceTypes || (type.Kind != TypeKind.Interface && type.Kind != TypeKind.Class);

			if (type.Kind == TypeKind.GenericParameter)
			{
				// ignore nullability until C# 8
				return $"{type.Name}";
			}

			if (type.Module == null)
			{
				// handle builtins

				switch (type.Name)
				{
					case "void":
						return type.Name;

					case "boolean":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"bool{(includeNullability && type.IsNullable ? "?" : "")}";

					case "byte":
					case "short":
					case "long":
					case "float":
					case "double":
					case "decimal":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"{type.Name}{(includeNullability && type.IsNullable ? "?" : "")}";

					case "integer":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"int{(includeNullability && type.IsNullable ? "?" : "")}";

					case "string":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return type.Name;

					case "datetime":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("System");

						return $"DateTime{(includeNullability && type.IsNullable ? "?" : "")}";

					case "date":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("System");

						return $"DateOnly{(includeNullability && type.IsNullable ? "?" : "")}";

					case "time":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("System");

						return $"TimeOnly{(includeNullability && type.IsNullable ? "?" : "")}";

					case "duration":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("System");

						return $"TimeSpan{(includeNullability && type.IsNullable ? "?" : "")}";

					case "array":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						return $"{Resolve(type.GenericArguments[0])}[]{(includeNullability && type.IsNullable ? "?" : "")}";

					case "file":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("Odachi.Abstractions");

						return $"IBlob{(includeNullability && type.IsNullable ? "?" : "")}";

					case "guid":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("System");

						return $"Guid{(includeNullability && type.IsNullable ? "?" : "")}";

					case "PagingOptions":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						Import("Odachi.Extensions.Collections");

						return $"PagingOptions{(includeNullability && type.IsNullable ? "?" : "")}";

					case "Page":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("Odachi.Extensions.Collections");

						return $"Page<{Resolve(type.GenericArguments[0])}>{(includeNullability && type.IsNullable ? "?" : "")}";

					case "tuple":
						if (type.GenericArguments?.Length < 1 || type.GenericArguments?.Length > 8)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return $"({string.Join(", ", type.GenericArguments.Select(t => Resolve(t)))}){(includeNullability && type.IsNullable ? "?" : "")}";

					case "oneof":
						if (type.GenericArguments?.Length < 2 || type.GenericArguments?.Length > 9)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("Odachi.Extensions.Primitives");

						return $"OneOf<{string.Join(", ", type.GenericArguments.Select(t => Resolve(t)))}>{(includeNullability && type.IsNullable ? "?" : "")}";

					case "ValidationState":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("Odachi.Validation");

						return $"ValidationState{(includeNullability && type.IsNullable ? "?" : "")}";

					case "ValidationLevel":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("Odachi.Validation");

						return $"ValidationLevel{(includeNullability && type.IsNullable ? "?" : "")}";

					case "ValidationMessage":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						Import("Odachi.Validation");

						return $"ValidationMessage{(includeNullability && type.IsNullable ? "?" : "")}";

					default:
						throw new NotSupportedException($"Undefined behavior for builtin '{type.Name}'");
				}
			}

			Import(type);

			return $"{type.Name}{(type.GenericArguments?.Length > 0 ? $"<{string.Join(", ", type.GenericArguments.Select(a => Resolve(a)))}>" : "")}{(includeNullability && type.IsNullable ? "?" : "")}";
		}

		#endregion
	}
}
