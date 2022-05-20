using Odachi.CodeGen.TypeScript.TypeHandlers;
using Odachi.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Odachi.CodeGen.TypeScript.StackinoUno.TypeHandlers
{
	public class StackinoUnoTypeHandler : ITypeHandler
	{
		public string Resolve(TypeScriptModuleContext context, TypeReference type, bool includeNullability = true, bool includeGenericArguments = true)
		{
			// accept only builtins
			if (type.Kind == TypeKind.GenericParameter || type.Module != null)
			{
				return null;
			}

			var nullableSuffix = includeNullability && type.IsNullable ? " | null" : "";

			switch (type.Name)
			{
				case "datetime":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

					context.Import("moment", "Moment");
					return $"Moment{nullableSuffix}";

				case "PagingOptions":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

					return $"{{ number: number, size?: number }}{nullableSuffix}";

				case "Page":
					if (type.GenericArguments?.Length != 1)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("@stackino/uno", "core");

					if (!includeGenericArguments)
						return "core.Page";

					return $"core.Page<{context.Resolve(type.GenericArguments[0])}>{nullableSuffix}";

				case "ValidationState":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("@stackino/uno", "validation");

					return $"validation.ValidationState{nullableSuffix}";

				default:
					return null;
			}
		}

		public string ResolveDefaultValue(TypeScriptModuleContext context, TypeReference type)
		{
			// accept only builtins
			if (type.Kind == TypeKind.GenericParameter || type.Module != null)
			{
				return null;
			}

			switch (type.Name)
			{
				case "datetime":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

					context.Import("moment", "* as Moment");
					return $"moment.invalid()";

				case "PagingOptions":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' is not generic");

					return $"{{ number: 0 }}";

				case "Page":
					if (type.GenericArguments?.Length != 1)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("@stackino/uno", "core");

					return $"new core.Page<{context.Resolve(type.GenericArguments[0])}>([], 0, 0)";

				case "ValidationState":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("@stackino/uno", "validation");

					return $"new validation.ValidationState()";

				default:
					return null;
			}
		}

		public string Factory(TypeScriptModuleContext context, TypeReference type)
		{
			const string privatePrefix = "_$$_";
			const string factoryPrefix = privatePrefix + "factory_";

			if (type.Kind == TypeKind.GenericParameter || type.Module != null)
			{
				return null;
			}

			var optHelper = $"{privatePrefix}opt";
			if (type.IsNullable)
			{
				context.Helper($"function {privatePrefix}opt<T>(T_factory: {{ create: (source: any) => T }}): {{ create: (source: any) => T | null }} {{ return {{ create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }}; }}");
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

			context.Helper($"function {privatePrefix}fail(message: string): never {{ throw new Error(message); }}");

			switch (type.Name)
			{
				case "datetime":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("moment", "* as moment");
					return MakeFactory(type.Name, $"(source: any): {context.Resolve(type, includeNullability: false)} => typeof source === 'string' ? moment(source) : {privatePrefix}fail(`Contract violation: expected datetime string, got '${{typeof(source)}}'`)");

				case "PagingOptions":
					return MakeFactory(type.Name, $"(source: any): {context.Resolve(type, includeNullability: false)} => typeof source === 'object' && source !== null ? source : {privatePrefix}fail(`Contract violation: expected paging options, got '${{typeof(source)}}'`)");

				case "Page":
					if (type.GenericArguments?.Length != 1)
						throw new NotSupportedException($"Builtin type '{type.Name}' requires exactly one generic argument");

					context.Import("@stackino/uno", "core");

					var pageFactory = MakeFactory(type.Name, $@"(source: any): core.Page<T> =>
		typeof source === 'object' && source !== null ?
			new core.Page<T>(
				{context.CreateExpression(new TypeReference(null, "array", TypeKind.Array, false, new TypeReference(null, "T", TypeKind.GenericParameter, false)), "source.data")},
				{context.CreateExpression(new TypeReference(null, "integer", TypeKind.Primitive, false), "source.number")},
				{context.CreateExpression(new TypeReference(null, "integer", TypeKind.Primitive, false), "source.count")}
			) :
			{privatePrefix}fail(`Contract violation: expected page, got '${{typeof(source)}}'`)", "T");

					return $"{pageFactory}({context.Factory(type.GenericArguments[0])})";

				case "ValidationState":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("@stackino/uno", "validation");

					return MakeFactory(type.Name, $@"(source: any): {context.Resolve(type, includeNullability: false)} => typeof source === 'object' && source !== null && typeof source.state === 'object' && source.state !== null ? new validation.ValidationState(source.state) : {privatePrefix}fail(`Contract violation: expected validation state, got '${{typeof(source)}}'`)");

				default:
					return null;
			}
		}
	}
}
