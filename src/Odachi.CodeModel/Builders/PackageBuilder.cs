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
			FieldDescriptors = new List<IFieldDescriptor>() { new DefaultFieldDescriptor() };
			MethodDescriptors = new List<IMethodDescriptor>() { new DefaultMethodDescriptor() };
			ParameterDescriptors = new List<IParameterDescriptor>() { new DefaultParameterDescriptor() };
			EnumDescriptors = new List<IEnumDescriptor>() { new DefaultEnumDescriptor() };
			EnumItemDescriptors = new List<IEnumItemDescriptor>() { new DefaultEnumItemDescriptor() };
			TypeMapper = new TypeMapper();
			ModulePath = ".";
		}
		public PackageContext(PackageContext copyFromContext, string newModulePath)
		{
			if (copyFromContext == null)
				throw new ArgumentNullException(nameof(copyFromContext));
			if (string.IsNullOrEmpty(newModulePath))
				throw new ArgumentNullException(nameof(newModulePath));

			GlobalDescriptor = copyFromContext.GlobalDescriptor;
			ObjectDescriptors = copyFromContext.ObjectDescriptors;
			ServiceDescriptors = copyFromContext.ServiceDescriptors;
			MethodDescriptors = copyFromContext.MethodDescriptors;
			ParameterDescriptors = copyFromContext.ParameterDescriptors;
			EnumDescriptors = copyFromContext.EnumDescriptors;
			EnumItemDescriptors = copyFromContext.EnumItemDescriptors;
			TypeMapper = copyFromContext.TypeMapper;
			ModulePath = newModulePath;
		}

		public IGlobalDescriptor GlobalDescriptor { get; }
		public IList<IObjectDescriptor> ObjectDescriptors { get; }
		public IList<IServiceDescriptor> ServiceDescriptors { get; }
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

	public class PackageBuilder : BuilderBase<PackageBuilder, Package>
	{
		private PackageBuilder(PackageBuilder parent, PackageContext context)
			: base(context, parent?.Name)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));

			Modules = parent.Modules;
		}
		public PackageBuilder(string name)
			: base(new PackageContext(), name)
		{
			Modules = new List<ModuleBuilder>();
		}

		public IList<ModuleBuilder> Modules { get; }

		public PackageBuilder Folder(string name, Action<PackageBuilder> configure)
		{
			var innerBuilder = new PackageBuilder(this, new PackageContext(Context, Context.MapPath(name)));

			configure(innerBuilder);

			return this;
		}

		public PackageBuilder Module(string name, Action<ModuleBuilder> configure)
		{
			var moduleBuilder = new ModuleBuilder(Context, Context.MapPath(name));

			configure(moduleBuilder);

			Modules.Add(moduleBuilder);

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

		public PackageBuilder Map<T>(TypeDefinition mapping)
		{
			return Map(typeof(T), mapping);
		}
		public PackageBuilder Map(Type type, TypeDefinition mapping)
		{
			Context.TypeMapper.Register(type, mapping);

			return this;
		}

		public override Package Build()
		{
			var result = new Package()
			{
				Name = Name,
			};

			// assume that autoregister adds new modules to the end of collection
			for (var i = 0; i < Modules.Count; i++)
			{
				var module = Modules[i];

				result.Modules.Add(module.Build());
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
				.Map(enumType, new FragmentTypeDefinition(builder.Context.MapPath(moduleName), fragmentName))
				.Module(moduleName, module => module
					.Enum(fragmentName, enumType, configure)
				);
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

			var fragmentName = builder.Context.GlobalDescriptor.GetFragmentName(builder.Context, objectType);
			var moduleName = builder.Context.GlobalDescriptor.GetModuleName(builder.Context, fragmentName);

			return builder
				.Map(objectType, new FragmentTypeDefinition(builder.Context.MapPath(moduleName), fragmentName))
				.Module(moduleName, module => module
					.Object(fragmentName, objectType, configure)
				);
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
				var members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
				foreach (var member in members)
				{
					if (member is FieldInfo field)
					{
						objectBuilder.Field(field.Name, ClrTypeReference.Create(field.FieldType), (objectType, field));
					}
					else if (member is PropertyInfo property)
					{
						objectBuilder.Field(property.Name, ClrTypeReference.Create(property.PropertyType), (objectType, property));
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
				.Map(objectType, new FragmentTypeDefinition(builder.Context.MapPath(moduleName), fragmentName))
				.Module(moduleName, module => module
					.Service(fragmentName, objectType, configure)
				);
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
				var members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
				foreach (var member in members)
				{
					if (member is MethodInfo method)
					{
						serviceBuilder.Method(method.Name, ClrTypeReference.Create(method.ReturnType), (objectType, method));
					}
				}

				configure?.Invoke(serviceBuilder);
			});
		}
	}
}
