using Odachi.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.Extensions.Collections;
using Odachi.Extensions.Primitives;

namespace Odachi.CodeModel.Mapping
{
	public class UnresolvedTypeEventArgs : EventArgs
	{
		public UnresolvedTypeEventArgs(Type type)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
		}

		public Type Type { get; }
	}

	public class TypeMapper
	{
		public TypeMapper()
		{
			_mapping = new Dictionary<Type, TypeDefinition>();

			Register(typeof(void), BuiltinTypeDefinition.Void);
			Register(typeof(bool), BuiltinTypeDefinition.Boolean);
			Register(typeof(byte), BuiltinTypeDefinition.Byte);
			Register(typeof(short), BuiltinTypeDefinition.Short);
			Register(typeof(int), BuiltinTypeDefinition.Integer);
			Register(typeof(long), BuiltinTypeDefinition.Long);
			Register(typeof(float), BuiltinTypeDefinition.Float);
			Register(typeof(double), BuiltinTypeDefinition.Double);
			Register(typeof(decimal), BuiltinTypeDefinition.Decimal);
			Register(typeof(string), BuiltinTypeDefinition.String);
			Register(typeof(DateOnly), BuiltinTypeDefinition.Date);
			Register(typeof(TimeOnly), BuiltinTypeDefinition.Time);
			Register(typeof(DateTime), BuiltinTypeDefinition.DateTime);
			Register(typeof(TimeSpan), BuiltinTypeDefinition.Duration);
			Register(typeof(IEnumerable<>), BuiltinTypeDefinition.Array);
			Register(typeof(IBlob), BuiltinTypeDefinition.File);
			Register(typeof(Guid), BuiltinTypeDefinition.Guid);

			Register(typeof(ValueTuple<>), BuiltinTypeDefinition.Tuple1);
			Register(typeof(ValueTuple<,>), BuiltinTypeDefinition.Tuple2);
			Register(typeof(ValueTuple<,,>), BuiltinTypeDefinition.Tuple3);
			Register(typeof(ValueTuple<,,,>), BuiltinTypeDefinition.Tuple4);
			Register(typeof(ValueTuple<,,,,>), BuiltinTypeDefinition.Tuple5);
			Register(typeof(ValueTuple<,,,,,>), BuiltinTypeDefinition.Tuple6);
			Register(typeof(ValueTuple<,,,,,,>), BuiltinTypeDefinition.Tuple7);
			Register(typeof(ValueTuple<,,,,,,,>), BuiltinTypeDefinition.Tuple8);

			Register(typeof(OneOf<,>), BuiltinTypeDefinition.OneOf2);
			Register(typeof(OneOf<,,>), BuiltinTypeDefinition.OneOf3);
			Register(typeof(OneOf<,,,>), BuiltinTypeDefinition.OneOf4);
			Register(typeof(OneOf<,,,,>), BuiltinTypeDefinition.OneOf5);
			Register(typeof(OneOf<,,,,,>), BuiltinTypeDefinition.OneOf6);
			Register(typeof(OneOf<,,,,,,>), BuiltinTypeDefinition.OneOf7);
			Register(typeof(OneOf<,,,,,,,>), BuiltinTypeDefinition.OneOf8);
			Register(typeof(OneOf<,,,,,,,,>), BuiltinTypeDefinition.OneOf9);
			Register(typeof(PagingOptions), BuiltinTypeDefinition.PagingOptions);
			Register(typeof(Page<>), BuiltinTypeDefinition.Page);
		}

		private IDictionary<Type, TypeDefinition> _mapping;

		public event EventHandler<UnresolvedTypeEventArgs> OnUnresolvedType;

		public void Register<T>(TypeDefinition mapping)
		{
			Register(typeof(T), mapping);
		}
		public void Register(Type type, TypeDefinition definition)
		{
			if (_mapping.ContainsKey(type))
				throw new InvalidOperationException($"CLR type '{type.FullName}' is already registered.");

			_mapping.Add(type, definition);
		}

		private bool TryGet(ref Type type, out TypeDefinition definition)
		{
			// exact lookup
			if (_mapping.TryGetValue(type, out definition))
			{
				return true;
			}

			// generic lookup
			if (type.GetTypeInfo().IsGenericType)
			{
				var genericTypeDefinition = type.GetGenericTypeDefinition();

				if (_mapping.TryGetValue(genericTypeDefinition, out definition))
				{
					if (type.GetGenericArguments().Length != definition.GenericArguments.Count)
						throw new InvalidOperationException($"Invalid number of generic arguments between '{type.FullName}' and '{definition.GetFullyQualifiedName()}'");

					return true;
				}
			}

			// IEnumerable<T> lookup
			var enumerableInterface = type.GetInterfaces().FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (enumerableInterface != null)
			{
				if (_mapping.TryGetValue(typeof(IEnumerable<>), out definition))
				{
					if (enumerableInterface.GetGenericArguments().Length != definition.GenericArguments.Count)
						throw new InvalidOperationException($"Invalid number of generic arguments between '{type.FullName}' and '{definition.GetFullyQualifiedName()}'");

					type = enumerableInterface;
					return true;
				}
			}

			definition = null;
			return false;
		}

		public TypeDefinition Get<T>(bool tryRegister = true)
		{
			return Get(typeof(T), tryRegister);
		}
		public TypeDefinition Get(Type type, bool tryRegister = true)
		{
			return Get(type, out _, tryRegister: tryRegister);
		}
		public TypeDefinition Get(Type type, out Type resolvedType, bool tryRegister = true)
		{
			if (!TryGet(ref type, out var mapping))
			{
				if (OnUnresolvedType == null || !tryRegister)
				{
					resolvedType = null;
					return null;
				}

				OnUnresolvedType(this, new UnresolvedTypeEventArgs(type));

				if (!TryGet(ref type, out mapping))
				{
					resolvedType = null;
					return null;
				}
			}

			resolvedType = type;
			return mapping;
		}
	}
}
