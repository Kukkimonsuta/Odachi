using Odachi.CodeGen.TypeScript.TypeHandlers;
using Odachi.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Odachi.CodeGen.TypeScript.StackinoDue.TypeHandlers
{
	public class StackinoDueTypeHandler : ITypeHandler
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

					if (context.PackageContext.Options.UseTemporal)
					{
						return null;
					}

					context.Import("luxon", "DateTime");
					return $"DateTime{nullableSuffix}";

				case "ValidationState":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("@odachi/validation", "ValidationState");

					return $"ValidationState{nullableSuffix}";

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

					if (context.PackageContext.Options.UseTemporal)
					{
						return null;
					}

					context.Import("luxon", "DateTime");
					return $"DateTime.invalid('default value')";

				case "ValidationState":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("@odachi/validation", "ValidationState");

					return $"new ValidationState()";

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

					if (context.PackageContext.Options.UseTemporal)
					{
						return null;
					}

					context.Import("luxon", "DateTime");
					return MakeFactory(type.Name, $"(source: any): {context.Resolve(type, includeNullability: false)} => typeof source === 'string' ? DateTime.fromISO(source, {{ setZone: true }}) : {privatePrefix}fail(`Contract violation: expected datetime string, got '${{typeof(source)}}'`)");

				case "ValidationState":
					if (type.GenericArguments?.Length > 0)
						throw new NotSupportedException($"Builtin type '{type.Name}' has invalid number of generic arguments");

					context.Import("@odachi/validation", "ValidationState");

					return MakeFactory(type.Name, $@"(source: any): {context.Resolve(type, includeNullability: false)} => typeof Array.isArray(source) ? new ValidationState(source) : {privatePrefix}fail(`Contract violation: expected validation state, got '${{typeof(source)}}'`)");

				default:
					return null;
			}
		}
	}
}
