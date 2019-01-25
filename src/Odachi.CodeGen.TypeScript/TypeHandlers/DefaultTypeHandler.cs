using Odachi.CodeGen.TypeScript.Internal;
using Odachi.CodeModel;
using System;
using System.Linq;

namespace Odachi.CodeGen.TypeScript.TypeHandlers
{
	public class DefaultTypeHandler : ITypeHandler
	{
		/// <summary>
		/// Returns string representation valid in code of a type reference.
		/// </summary>
		public string Resolve(TypeScriptModuleContext context, TypeReference type, bool includeNullability = true, bool includeGenericArguments = true)
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

					case "byte":
					case "short":
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

						return $"Date{nullableSuffix}";

					case "array":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						if (!includeGenericArguments)
							return "Array";

						return $"Array<{context.Resolve(type.GenericArguments[0])}>{nullableSuffix}";

					case "file":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"File{nullableSuffix}";

					case "Page":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						context.Import("Page", "@odachi/collections");
						return $"Page<{context.Resolve(type.GenericArguments[0])}>{nullableSuffix}";

					case "PagingOptions":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						context.Import("PagingOptions", "@odachi/collections");
						return $"PagingOptions{nullableSuffix}";

					case "Tuple":
						if (type.GenericArguments?.Length < 1 || type.GenericArguments?.Length > 8)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						if (!includeGenericArguments)
							throw new InvalidOperationException("Cannot resolve tuple without generic arguments");

						return $"[{string.Join(", ", type.GenericArguments.Select(t => context.Resolve(t)))}]{nullableSuffix}";

					case "OneOf":
						if (type.GenericArguments?.Length < 2 || type.GenericArguments?.Length > 9)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						if (!includeGenericArguments)
							throw new InvalidOperationException("Cannot resolve oneof without generic arguments");

						if (includeNullability && type.GenericArguments.Any(a => a.IsNullable))
							nullableSuffix = " | null";

						return $"{string.Join(" | ", type.GenericArguments.Select(t => context.Resolve(t, includeNullability: false)))}{nullableSuffix}";

					default:
						return null;
				}
			}
			else
			{
				// handle modules

				context.Import(type);

				return $"{type.Name}{(includeGenericArguments && type.GenericArguments?.Length > 0 ? $"<{string.Join(", ", type.GenericArguments.Select(a => context.Resolve(a)))}>" : "")}{nullableSuffix}";
			}
		}

		public string ResolveDefaultValue(TypeScriptModuleContext context, TypeReference type)
		{
			if (type.Kind == TypeKind.GenericParameter)
			{
				return null;
			}

			if (type.Module == null)
			{
				// handle builtins

				switch (type.Name)
				{
					case "boolean":
						return "false";

					case "string":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return "''";

					case "byte":
					case "short":
					case "integer":
					case "long":
					case "float":
					case "double":
					case "decimal":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return "0";

					case "datetime":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return "new Date(NaN)";

					case "array":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						return "[]";

					case "Tuple":
						if (type.GenericArguments?.Length < 1 || type.GenericArguments?.Length > 8)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return $"[{string.Join(", ", type.GenericArguments.Select(t => context.ResolveDefaultValue(t)))}]";

					case "Page":
						if (type.GenericArguments?.Length != 1)
							throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

						context.Import("Page", "@odachi/collections");
						return $"new Page<{context.Resolve(type.GenericArguments[0])}>([], 0, 0)";

					case "PagingOptions":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

						return $"{{ page: 0 }}";

					case "OneOf":
						if (type.GenericArguments?.Length < 2 || type.GenericArguments?.Length > 9)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						if (type.GenericArguments.Any(a => a.IsNullable))
							return "null";

						return null;

					default:
						return null;
				}
			}
			else
			{
				if (type.Kind == TypeKind.Enum)
				{
					return "0";
				}

				return null;
			}
		}

		/// <summary>
		/// Returns reference to a factory for given type.
		/// </summary>
		public string Factory(TypeScriptModuleContext context, TypeReference type)
		{
			const string privatePrefix = "_$$_";
			const string factoryPrefix = privatePrefix + "factory_";

			//if (type.GenericArguments.Any(t => t.Kind == TypeKind.GenericParameter))
			//{
			//	throw new InvalidOperationException("Cannot create factory for open generic type");
			//}

			if (type.Kind == TypeKind.GenericParameter)
			{
				// assume factory exists
				return $"{type.Name}_factory";
			}

			var optHelper = $"{privatePrefix}opt";
			if (type.IsNullable)
			{
				context.Helper($"function {privatePrefix}opt<T>(T_factory: {{ create: (source: any) => T }}): {{ create: (source: any) => T | null }} {{ return {{ create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }}; }}");
			}

			string GetUndHelper()
			{
				context.Helper($"function {privatePrefix}und<T>(T_factory: {{ create: (source: any) => T }}): {{ create: (source: any) => T | undefined }} {{ return {{ create: (source: any): T | undefined => source === undefined || source === null ? undefined : T_factory.create(source) }}; }}");
				return $"{privatePrefix}und";
			}

			string MakeFactory(string name, string factory, string[] genericParameterNames, string noValueHelper = null)
			{
				noValueHelper = noValueHelper ?? optHelper;

				var factoryName = $"{factoryPrefix}{name}";

				if (genericParameterNames.Length != type.GenericArguments.Length)
					throw new InvalidOperationException("Generic parameter count mismatch");

				if (genericParameterNames.Any())
				{
					var genericParameters = string.Join(", ", genericParameterNames);
					var arguments = string.Join(", ", genericParameterNames.Select(n => $"{n}_factory: {{ create: (source: any) => {n} }}"));

					context.Helper($"function {factoryName}<{genericParameters}>({arguments}) {{ return {{ create: {factory} }}; }}");

					if (!type.IsNullable)
					{
						return factoryName;
					}

					var optFactoryName = $"{factoryName}_opt";
					var argumentForward = string.Join(", ", genericParameterNames.Select(n => $"{n}_factory"));

					context.Helper($"function {optFactoryName}<{genericParameters}>({arguments}) {{ return {optHelper}({factoryName}({argumentForward})); }}");
					return optFactoryName;
				}
				else
				{
					context.Helper($"const {factoryName} = {{ create: {factory} }};");

					if (!type.IsNullable)
					{
						return factoryName;
					}

					var optFactoryName = $"{factoryName}_opt";

					context.Helper($"const {optFactoryName} = {optHelper}({factoryName});");
					return optFactoryName;
				}
			}

			if (type.Module == null)
			{
				context.Helper("function fail(message: string): never { throw new Error(message); }");

				switch (type.Name)
				{
					case "boolean":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return MakeFactory(
							type.Name,
							$"(source: any): {context.Resolve(type, includeNullability: false)} => typeof source === 'boolean' ? source : fail(`Contract violation: expected boolean, got \\'{{typeof(source)}}\\'`)",
							Array.Empty<string>()
						);

					case "string":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return MakeFactory(
							type.Name,
							$"(source: any): {context.Resolve(type, includeNullability: false)} => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \\'{{typeof(source)}}\\'`)",
							Array.Empty<string>()
						);

					case "byte":
					case "short":
					case "integer":
					case "long":
					case "float":
					case "double":
					case "decimal":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return MakeFactory(
							"number",
							$"(source: any): {context.Resolve(type, includeNullability: false)} => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \\'{{typeof(source)}}\\'`)",
							Array.Empty<string>()
						);

					case "datetime":
						if (type.GenericArguments?.Length > 0)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						return MakeFactory(
							type.Name,
							$"(source: any): {context.Resolve(type, includeNullability: false)} => typeof source === 'string' ? new Date(source) : fail(`Contract violation: expected datetime string, got \\'{{typeof(source)}}\\'`)",
							Array.Empty<string>()
						);

					case "array":
						{
							if (type.GenericArguments?.Length != 1)
								throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

							var arrayFactory = MakeFactory(
								type.Name,
								$@"(source: any): Array<T> => Array.isArray(source) ? source.map((item: any) => T_factory.create(item)) : fail(`Contract violation: expected array, got \\'{{typeof(source)}}\\'`)",
								new[] { "T" }
							);

							return $"{arrayFactory}({context.Factory(type.GenericArguments[0])})";
						}

					case "Page":
						{
							if (type.GenericArguments?.Length != 1)
								throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

							var numberFactory = context.Factory(new TypeReference(null, "integer", TypeKind.Primitive, false));

							var pageFactory = MakeFactory(
								type.Name,
								$@"(source: any): Page<T> => new Page(Array.isArray(source.data) ? source.data.map((item: any) => T_factory.create(item)) : fail(`Contract violation: expected array, got \\'{{typeof(source)}}\\'`), {numberFactory}.create(source.number), {numberFactory}.create(source.count))",
								new[] { "T" }
							);

							context.Import("Page", "@odachi/collections");
							return $"{pageFactory}({context.Factory(type.GenericArguments[0])})";
						}

					case "PagingOptions":
						{
							if (type.GenericArguments?.Length > 0)
								throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

							var numberFactory = context.Factory(new TypeReference(null, "integer", TypeKind.Primitive, false));

							var numberFactoryUnd = $"{numberFactory}_und";
							context.Helper($"const {numberFactoryUnd} = {GetUndHelper()}({numberFactory});");

							context.Import("PagingOptions", "@odachi/collections");
							return MakeFactory(
								type.Name,
								$@"(source: any): PagingOptions => ({{ page: {numberFactory}.create(source.page), size: {numberFactoryUnd}.create(source.size), offset: {numberFactoryUnd}.create(source.offset), maximumCount: {numberFactoryUnd}.create(source.maximumCount) }})",
								Array.Empty<string>()
							);
						}

					case "Tuple":
						if (type.GenericArguments?.Length < 1 || type.GenericArguments?.Length > 8)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						var tupleHelperGenericArguments = new string[type.GenericArguments.Length];
						var tupleHelperBody = $"[";
						for (var i = 0; i < type.GenericArguments.Length; i++)
						{
							var genericArgumentName = $"T{i + 1}";

							tupleHelperGenericArguments[i] = genericArgumentName;
							tupleHelperBody += $"{genericArgumentName}_factory.create(source.item{i + 1})";
							if (i != type.GenericArguments.Length - 1)
							{
								tupleHelperBody += ", ";
							}
						}
						tupleHelperBody += "]";

						var tupleFactory = MakeFactory($"tuple{type.GenericArguments.Length}", $"(source: any): [{string.Join(", ", tupleHelperGenericArguments)}] => {tupleHelperBody}", tupleHelperGenericArguments);

						return $"{tupleFactory}({string.Join(", ", type.GenericArguments.Select(a => context.Factory(a)))})";

					case "OneOf":
						if (type.GenericArguments?.Length < 2 || type.GenericArguments?.Length > 9)
							throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

						var oneOfHelperGenericArguments = new string[type.GenericArguments.Length];
						var oneOfHelperBody = "switch (source.index) { ";
						for (var i = 0; i < type.GenericArguments.Length; i++)
						{
							var genericArgumentName = $"T{i + 1}";

							oneOfHelperGenericArguments[i] = genericArgumentName;
							oneOfHelperBody += $"case {i + 1}: return {genericArgumentName}_factory.create(source.option{i + 1}); ";
						}
						oneOfHelperBody += "default: return fail(`Contract violation: cannot handle OneOf index ${source.index}`); ";
						oneOfHelperBody += "}";

						var oneOfFactory = MakeFactory($"oneof{type.GenericArguments.Length}", $"(source: any): {string.Join(" | ", oneOfHelperGenericArguments)} => {{ {oneOfHelperBody} }}", oneOfHelperGenericArguments);

						return $"{oneOfFactory}({string.Join(", ", type.GenericArguments.Select(a => context.Factory(a)))})";

					default:
						return null;
				}
			}
			else
			{
				context.Import(type);

				var factoryBase = context.Resolve(type, includeNullability: false, includeGenericArguments: false);
				if (type.GenericArguments.Any())
				{
					factoryBase = $"{factoryBase}.create({string.Join(", ", type.GenericArguments.Select(a => context.Factory(a)))})";
				}

				if (type.IsNullable)
				{
					return $"{optHelper}({factoryBase})";
				}
				else
				{
					return factoryBase;
				}
			}
		}
	}
}
