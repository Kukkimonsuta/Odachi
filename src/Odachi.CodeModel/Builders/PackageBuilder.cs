using Odachi.CodeModel.Description;
using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;

namespace Odachi.CodeModel.Builders
{
	public class PackageContext
	{
		public PackageContext()
		{
			GlobalDescriptor = new DefaultGlobalDescriptor();
			ObjectDescriptors = new List<IObjectDescriptor>() { new DefaultObjectDescriptor() };
			ServiceDescriptors = new List<IServiceDescriptor>() { new DefaultServiceDescriptor() };
			ConstantDescriptors = new List<IConstantDescriptor>() { new DefaultConstantDescriptor() };
			FieldDescriptors = new List<IFieldDescriptor>() { new DefaultFieldDescriptor() };
			MethodDescriptors = new List<IMethodDescriptor>() { new DefaultMethodDescriptor() };
			ParameterDescriptors = new List<IParameterDescriptor>() { new DefaultParameterDescriptor() };
			EnumDescriptors = new List<IEnumDescriptor>() { new DefaultEnumDescriptor() };
			EnumItemDescriptors = new List<IEnumItemDescriptor>() { new DefaultEnumItemDescriptor() };
			TypeMapper = new TypeMapper();
			ModulePath = ".";
		}
		public PackageContext(PackageContext parentContext, string newModulePath)
		{
			if (parentContext == null)
				throw new ArgumentNullException(nameof(parentContext));
			if (string.IsNullOrEmpty(newModulePath))
				throw new ArgumentNullException(nameof(newModulePath));

			GlobalDescriptor = parentContext.GlobalDescriptor;
			ObjectDescriptors = parentContext.ObjectDescriptors;
			ServiceDescriptors = parentContext.ServiceDescriptors;
			ConstantDescriptors = parentContext.ConstantDescriptors;
			FieldDescriptors = parentContext.FieldDescriptors;
			MethodDescriptors = parentContext.MethodDescriptors;
			ParameterDescriptors = parentContext.ParameterDescriptors;
			EnumDescriptors = parentContext.EnumDescriptors;
			EnumItemDescriptors = parentContext.EnumItemDescriptors;
			TypeMapper = parentContext.TypeMapper;
			ModulePath = newModulePath;
		}

		public IGlobalDescriptor GlobalDescriptor { get; }
		public IList<IObjectDescriptor> ObjectDescriptors { get; }
		public IList<IServiceDescriptor> ServiceDescriptors { get; }
		public IList<IConstantDescriptor> ConstantDescriptors { get; }
		public IList<IFieldDescriptor> FieldDescriptors { get; }
		public IList<IMethodDescriptor> MethodDescriptors { get; }
		public IList<IParameterDescriptor> ParameterDescriptors { get; }
		public IList<IEnumDescriptor> EnumDescriptors { get; }
		public IList<IEnumItemDescriptor> EnumItemDescriptors { get; }
		public TypeMapper TypeMapper { get; }
		public string ModulePath { get; }

		public string MapPath(string path)
		{
			return Path.Combine(ModulePath, path);
		}
	}

	public class PackageBuilder : FragmentBuilderBase<PackageBuilder, Package>
	{
		private PackageBuilder(PackageBuilder parent, PackageContext context)
			: base(context, parent.Name)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));

			Enums = parent.Enums;
			Objects = parent.Objects;
			Services = parent.Services;
		}
		public PackageBuilder(string name)
			: base(new PackageContext(), name)
		{
			Enums = new List<EnumBuilder>();
			Objects = new List<ObjectBuilder>();
			Services = new List<ServiceBuilder>();
		}

		public IList<EnumBuilder> Enums { get; set; }
		public IList<ObjectBuilder> Objects { get; set; }
		public IList<ServiceBuilder> Services { get; set; }

		public PackageBuilder Folder(string name, Action<PackageBuilder> configure)
		{
			var innerBuilder = new PackageBuilder(this, new PackageContext(Context, Context.MapPath(name)));

			configure(innerBuilder);

			return this;
		}

		public PackageBuilder Enum(string name, Action<EnumBuilder> configure)
		{
			return Enum(name, null, configure);
		}
		public PackageBuilder Enum(string name, Type type, Action<EnumBuilder> configure)
		{
			var enumBuilder = new EnumBuilder(Context, name, source: type);

			configure(enumBuilder);

			Enums.Add(enumBuilder);

			return this;
		}

		public PackageBuilder Object(string name, Action<ObjectBuilder> configure)
		{
			return Object(name, null, null, configure);
		}
		public PackageBuilder Object(string name, Type type, Action<ObjectBuilder> configure)
		{
			return Object(name, null, type, configure);
		}
		public PackageBuilder Object(string name, IReadOnlyList<string> genericArguments, Action<ObjectBuilder> configure)
		{
			return Object(name, genericArguments, null, configure);
		}
		public PackageBuilder Object(string name, IReadOnlyList<string> genericArguments, Type type, Action<ObjectBuilder> configure)
		{
			var objectBuilder = new ObjectBuilder(Context, name, genericArguments, source: type);

			configure(objectBuilder);

			Objects.Add(objectBuilder);

			return this;
		}

		public PackageBuilder Service(string name, Action<ServiceBuilder> configure)
		{
			return Service(name, null, configure);
		}
		public PackageBuilder Service(string name, Type type, Action<ServiceBuilder> configure)
		{
			var serviceBuilder = new ServiceBuilder(Context, name, source: type);

			configure(serviceBuilder);

			Services.Add(serviceBuilder);

			return this;
		}

		public PackageBuilder AutoRegister<TResult>(Func<PackageBuilder, Type, TResult> action)
		{
			return AutoRegister((p, t) =>
			{
				action(p, t);
			});
		}
		public PackageBuilder AutoRegister(Action<PackageBuilder, Type> action)
		{
			Context.TypeMapper.OnUnresolvedType += (sender, args) =>
			{
				action(this, args.Type);
			};

			return this;
		}

		public PackageBuilder AutoRegister<TResult>(Func<Type, bool> condition, Func<PackageBuilder, Type, TResult> action)
		{
			return AutoRegister(condition, (p, t) =>
			{
				action(p, t);
			});
		}
		public PackageBuilder AutoRegister(Func<Type, bool> condition, Action<PackageBuilder, Type> action)
		{
			Context.TypeMapper.OnUnresolvedType += (sender, args) =>
			{
				if (!condition(args.Type))
					return;

				action(this, args.Type);
			};

			return this;
		}

		public PackageBuilder Map<T>(TypeDefinition definition)
		{
			return Map(typeof(T), definition);
		}
		public PackageBuilder Map(Type type, TypeDefinition definition)
		{
			Context.TypeMapper.Register(type, definition);

			return this;
		}

		public PackageBuilder MapFragment<T>(string moduleName, string fragmentName)
		{
			return MapFragment(typeof(T), moduleName, fragmentName);
		}
		public PackageBuilder MapFragment(Type type, string moduleName, string fragmentName)
		{
			var genericArgumentDefinition = type.GetGenericArguments().Select(a => new GenericArgumentDefinition(a.Name)).ToArray();

			return Map(type, new FragmentTypeDefinition(Context.MapPath(moduleName), fragmentName, genericArgumentDefinition));
		}

		public override Package Build()
		{
			var result = new Package()
			{
				Name = Name,
			};

			for (var i = 0; i < Enums.Count; i++)
			{
				var @enum = Enums[i];

				result.Enums.Add(@enum.Build());
			}
			for (var i = 0; i < Objects.Count; i++)
			{
				var @object = Objects[i];

				result.Objects.Add(@object.Build());
			}
			for (var i = 0; i < Services.Count; i++)
			{
				var service = Services[i];

				result.Services.Add(service.Build());
			}

			foreach (var hint in Hints)
			{
				result.Hints.Add(hint);
			}

			return result;
		}

		#region Static members

		public static Package Build(string name, Action<PackageBuilder> configure)
		{
			var builder = new PackageBuilder(name);

			configure(builder);

			return builder.Build();
		}

		#endregion
	}

	public static class PackageBuilderExtensions
	{
		/// <summary>
		/// Shortcut for creating a module with single enum fragment from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Enum<T>(this PackageBuilder builder, Action<EnumBuilder> configure)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));

			return Module_Enum(builder, typeof(T), configure);
		}
		/// <summary>
		/// Shortcut for creating a module with single enum fragment from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Enum(this PackageBuilder builder, Type enumType, Action<EnumBuilder> configure)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (enumType == null)
				throw new ArgumentNullException(nameof(enumType));
			if (!enumType.GetTypeInfo().IsEnum)
				throw new ArgumentException($"Parameter `enumType` must be enum type");
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));

			var fragmentName = builder.Context.GlobalDescriptor.GetFragmentName(builder.Context, enumType);
			var moduleName = builder.Context.GlobalDescriptor.GetModuleName(builder.Context, fragmentName);

			return builder
				.MapFragment(enumType, moduleName, fragmentName)
				.Enum(fragmentName, enumType, configure);
		}

		/// <summary>
		/// Shortcut for creating a module with single enum fragment and all its enum items from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Enum_Default<T>(this PackageBuilder builder, Action<EnumBuilder> configure = null)
			where T : struct
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			return Module_Enum_Default(builder, typeof(T), configure: configure);
		}
		/// <summary>
		/// Shortcut for creating a module with single enum fragment and all its enum items from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Enum_Default(this PackageBuilder builder, Type enumType, Action<EnumBuilder> configure = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (enumType == null)
				throw new ArgumentNullException(nameof(enumType));
			if (!enumType.GetTypeInfo().IsEnum)
				throw new ArgumentException($"Parameter `enumType` must be enum type");

			return builder.Module_Enum(enumType, enumBuilder =>
			{
				foreach (var item in Enum.GetValues(enumType))
				{
					enumBuilder.Item(item.ToString(), Convert.ToInt32(item));
				}

				configure?.Invoke(enumBuilder);
			});
		}

		/// <summary>
		/// Shortcut for creating a module with single object fragment from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Object<T>(this PackageBuilder builder, Action<ObjectBuilder> configure)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));

			return Module_Object(builder, typeof(T), configure);
		}
		/// <summary>
		/// Shortcut for creating a module with single object fragment from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Object(this PackageBuilder builder, Type objectType, Action<ObjectBuilder> configure)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (objectType == null)
				throw new ArgumentNullException(nameof(objectType));
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));

			var objectTypeInfo = objectType.GetTypeInfo();

			var fragmentName = builder.Context.GlobalDescriptor.GetFragmentName(builder.Context, objectType);
			var moduleName = builder.Context.GlobalDescriptor.GetModuleName(builder.Context, fragmentName);
			var genericArguments = objectTypeInfo.ContainsGenericParameters? objectTypeInfo.GenericTypeParameters.Select(p => p.Name).ToArray() : null;

			return builder
				.MapFragment(objectType, moduleName, fragmentName)
				.Object(fragmentName, genericArguments, objectType, configure);
		}

		/// <summary>
		/// Shortcut for creating a module with single object fragment and all its fields and properties from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Object_Default<T>(this PackageBuilder builder, Action<ObjectBuilder> configure = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			return Module_Object_Default(builder, typeof(T), configure: configure);
		}
		/// <summary>
		/// Shortcut for creating a module with single object fragment and all its fields and properties from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Object_Default(this PackageBuilder builder, Type objectType, Action<ObjectBuilder> configure = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (objectType == null)
				throw new ArgumentNullException(nameof(objectType));

			return builder.Module_Object(objectType, objectBuilder =>
			{
				var members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				foreach (var member in members)
				{
					if (member is FieldInfo field)
					{
						if (field.IsLiteral && !field.IsInitOnly)
						{
							var value = field.GetRawConstantValue();

							objectBuilder.Constant(field.Name, ClrTypeReference.Create(field.FieldType, isNullable: value == null), value, (objectType, field));
						}
						else if (!field.IsStatic)
						{
							objectBuilder.Field(field.Name, ClrTypeReference.Create(field.FieldType), (objectType, field));
						}
					}
					else if (member is PropertyInfo property)
					{
						if (!(property.GetMethod?.IsStatic ?? false) && !(property.SetMethod?.IsStatic ?? false))
						{
							objectBuilder.Field(property.Name, ClrTypeReference.Create(property.PropertyType), (objectType, property));
						}
					}
				}

				configure?.Invoke(objectBuilder);
			});
		}

		/// <summary>
		/// Shortcut for creating a module with single service fragment from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Service<T>(this PackageBuilder builder, Action<ServiceBuilder> configure)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));

			return Module_Service(builder, typeof(T), configure);
		}
		/// <summary>
		/// Shortcut for creating a module with single service fragment from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Service(this PackageBuilder builder, Type objectType, Action<ServiceBuilder> configure)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (objectType == null)
				throw new ArgumentNullException(nameof(objectType));
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));

			var fragmentName = builder.Context.GlobalDescriptor.GetFragmentName(builder.Context, objectType);
			var moduleName = builder.Context.GlobalDescriptor.GetModuleName(builder.Context, fragmentName);

			return builder
				.MapFragment(objectType, moduleName, fragmentName)
				.Service(fragmentName, objectType, configure);
		}

		/// <summary>
		/// Shortcut for creating a module with single service fragment and all its fields and properties from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Service_Default<T>(this PackageBuilder builder, Action<ServiceBuilder> configure = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			return Module_Service_Default(builder, typeof(T), configure: configure);
		}
		/// <summary>
		/// Shortcut for creating a module with single service fragment and all its fields and properties from specified .NET type.
		/// </summary>
		public static PackageBuilder Module_Service_Default(this PackageBuilder builder, Type objectType, Action<ServiceBuilder> configure = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (objectType == null)
				throw new ArgumentNullException(nameof(objectType));

			return builder.Module_Service(objectType, serviceBuilder =>
			{
				var members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				foreach (var member in members)
				{
					// ignore GetType, GetHashCode, Equals, ToString
					if (member.DeclaringType == typeof(object))
						continue;

					if (member is FieldInfo field)
					{
						if (field.IsLiteral && !field.IsInitOnly)
						{
							var value = field.GetRawConstantValue();

							serviceBuilder.Constant(field.Name, ClrTypeReference.Create(field.FieldType, isNullable: value == null), value, (objectType, field));
						}
					}
					else if (member is MethodInfo method)
					{
						if (!method.IsStatic)
						{
							serviceBuilder.Method(method.Name, ClrTypeReference.Create(method.ReturnType), (objectType, method));
						}
					}
				}

				configure?.Invoke(serviceBuilder);
			});
		}
	}
}
