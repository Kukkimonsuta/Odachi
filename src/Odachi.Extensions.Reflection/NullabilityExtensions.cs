using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Odachi.Extensions.Reflection
{
	public static class NullabilityExtensions
	{
		// https://github.com/dotnet/roslyn/blob/master/docs/features/nullable-metadata.md
		private const string _nullableAttributeFullName = "System.Runtime.CompilerServices.NullableAttribute";
		private const string _nullableContextAttributeFullName = "System.Runtime.CompilerServices.NullableContextAttribute";
		private const string _maybeNullAttributeFullName = "System.Diagnostics.CodeAnalysis.MaybeNullAttribute";

		private static byte? GetNullableAttributeFlag(Attribute[] attributes, int index = 0)
		{
			if (attributes.FirstOrDefault(a => a.GetType().FullName == _nullableAttributeFullName) is Attribute attribute)
			{
				var attributeType = attribute.GetType();
				var flagsField = attributeType.GetField("NullableFlags");

				if (flagsField?.GetValue(attribute) is byte[] flags)
				{
					if (flags.Length <= 0)
					{
						return null;
					}
					else if (flags.Length == 1)
					{
						// optimized value, all bits are the same
						return flags.Single();
					}
					else
					{
						return flags[index];
					}
				}
			}

			return null;
		}

		#region Type

		public static bool? IsNonNullableContext(this Type type)
		{
			if (Attribute.GetCustomAttributes(type).FirstOrDefault(a => a.GetType().FullName == _nullableContextAttributeFullName) is Attribute contextAttribute)
			{
				var attributeType = contextAttribute.GetType();
				var flagField = attributeType.GetField("Flag");

				if (flagField?.GetValue(contextAttribute) is byte flag)
				{
					return flag == 1;
				}

				// attribute was found, but is invalid => silently ignore
			}

			// TODO: generic parameters?

			return type.DeclaringType?.IsNonNullableContext();
		}

		public static bool IsNonNullableValueType(this Type type)
		{
			return type.IsValueType && Nullable.GetUnderlyingType(type) == null;
		}

		private static Type? GetGenericArgumentByOrdinal(Type type, int n, out int nullableFlagIndex)
		{
			var r = n;

			// TODO: Tuple: the representation of the underlying constructed type

			// Reference type: the nullability (0, 1, or 2), followed by the representation of the type arguments in order including containing types
			// Generic value type: 0, followed by the representation of the type arguments in order including containing types
			// Array: the nullability (0, 1, or 2), followed by the representation of the element type
			nullableFlagIndex = 0;

			return GetGenericArgumentByOrdinal(type, ref r, ref nullableFlagIndex);
		}
		private static Type? GetGenericArgumentByOrdinal(Type type, ref int remaining, ref int nullableFlagIndex)
		{
			static void IncrementNullableFlagIndexFor(Type type, ref int nullableFlagIndex)
			{
				if (type is { IsValueType: true, IsGenericType: false })
				{
					return;
				}
				if (Nullable.GetUnderlyingType(type) != null)
				{
					return;
				}

				nullableFlagIndex += 1;
			}

			// treat arrays as having single generic argument
			if (type.IsArray)
			{
				var current = type.GetElementType() ?? throw new InvalidOperationException("Failed to determine array element type");
				IncrementNullableFlagIndexFor(current, ref nullableFlagIndex);

				if (remaining == 0)
				{
					return current;
				}

				remaining -= 1;

				return GetGenericArgumentByOrdinal(current, ref remaining, ref nullableFlagIndex);
			}

			// skip value nullable wrapper
			if (Nullable.GetUnderlyingType(type) is {} nullableUnderlyingType)
			{
				return GetGenericArgumentByOrdinal(nullableUnderlyingType, ref remaining, ref nullableFlagIndex);
			}

			var genericArguments = type.GetGenericArguments();
			for (var i = 0; i < genericArguments.Length; i++)
			{
				var current = genericArguments[i];
				IncrementNullableFlagIndexFor(current, ref nullableFlagIndex);

				if (remaining == 0)
				{
					return current;
				}

				remaining -= 1;

				var result = GetGenericArgumentByOrdinal(current, ref remaining, ref nullableFlagIndex);
				if (result != null && remaining == 0)
				{
					return result;
				}
			}

			// not found
			return null;
		}

		#endregion

		#region MethodBase

		public static bool? IsNonNullableContext(this MethodBase methodBase)
		{
			if (Attribute.GetCustomAttributes(methodBase).FirstOrDefault(a => a.GetType().FullName == _nullableContextAttributeFullName) is Attribute contextAttr)
			{
				var attributeType = contextAttr.GetType();
				var flagField = attributeType.GetField("Flag");

				if (flagField?.GetValue(contextAttr) is byte flag)
				{
					return flag == 1;
				}

				// attribute was found, but is invalid => silently ignore
			}

			return methodBase.DeclaringType?.IsNonNullableContext();
		}

		#endregion

		#region FieldInfo

		public static bool IsValueType(this FieldInfo fieldInfo)
		{
			return fieldInfo.FieldType.IsValueType;
		}

		public static bool IsNonNullableValueType(this FieldInfo fieldInfo)
		{
			return fieldInfo.FieldType.IsNonNullableValueType();
		}

		public static bool IsNonNullableReferenceType(this FieldInfo fieldInfo)
		{
			// value type => not a non nullable reference type
			if (fieldInfo.IsValueType())
			{
				return false;
			}

			// MaybeNullAttribute => not a non nullable reference type
			var isMaybeNull = fieldInfo.CustomAttributes.Any(a => a.AttributeType.FullName == _maybeNullAttributeFullName);
			//PropertyInfo p => p.GetMethod?.ReturnParameter?.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == MaybeNullAttributeFullName) != null,
			if (isMaybeNull)
			{
				return false;
			}

			// check member
			var flag = GetNullableAttributeFlag(Attribute.GetCustomAttributes(fieldInfo));
			if (flag != null)
			{
				return flag == 1;
			}

			// check declaring type
			var declaringType = fieldInfo.DeclaringType;
			if (declaringType?.IsNonNullableContext() == true)
			{
				return true;
			}

			return false;
		}

		public static bool IsNonNullable(this FieldInfo fieldInfo)
		{
			return fieldInfo.IsNonNullableValueType() || fieldInfo.IsNonNullableReferenceType();
		}

		#endregion

		#region FieldInfo generic arguments

		public static bool IsGenericArgumentValueType(this FieldInfo fieldInfo, int index)
		{
			if (fieldInfo == null)
				throw new ArgumentNullException(nameof(fieldInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(fieldInfo.FieldType, index, out var _);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			return argument.IsValueType;
		}

		public static bool IsGenericArgumentNonNullableValueType(this FieldInfo fieldInfo, int index)
		{
			if (fieldInfo == null)
				throw new ArgumentNullException(nameof(fieldInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(fieldInfo.FieldType, index, out _);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			return argument.IsNonNullableValueType();
		}

		public static bool IsGenericArgumentNonNullableReferenceType(this FieldInfo fieldInfo, int index)
		{
			if (fieldInfo == null)
				throw new ArgumentNullException(nameof(fieldInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(fieldInfo.FieldType, index, out var nullableFlagIndex);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (argument.IsValueType)
			{
				return false;
			}

			// check member
			var flag = GetNullableAttributeFlag(Attribute.GetCustomAttributes(fieldInfo), nullableFlagIndex);
			if (flag != null)
			{
				return flag == 1;
			}

			// check declaring type
			var declaringType = fieldInfo.DeclaringType;
			if (declaringType?.IsNonNullableContext() == true)
			{
				return true;
			}

			return false;
		}

		public static bool IsGenericArgumentNonNullable(this FieldInfo fieldInfo, int index)
		{
			if (fieldInfo == null)
				throw new ArgumentNullException(nameof(fieldInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			return fieldInfo.IsGenericArgumentNonNullableValueType(index) || fieldInfo.IsGenericArgumentNonNullableReferenceType(index);
		}

		#endregion

		#region PropertyInfo

		public static bool IsValueType(this PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));

			return propertyInfo.PropertyType.IsValueType;
		}

		public static bool IsNonNullableValueType(this PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));

			return propertyInfo.PropertyType.IsNonNullableValueType();
		}

		public static bool IsNonNullableReferenceType(this PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));

			// value type => not a non nullable reference type
			if (propertyInfo.IsValueType())
			{
				return false;
			}

			// MaybeNullAttribute => not a non nullable reference type
			var isMaybeNull = propertyInfo.GetMethod?.ReturnParameter?.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == _maybeNullAttributeFullName) != null;
			if (isMaybeNull)
			{
				return false;
			}

			// check member
			var flag = GetNullableAttributeFlag(Attribute.GetCustomAttributes(propertyInfo));
			if (flag != null)
			{
				return flag == 1;
			}

			// check declaring type
			var declaringType = propertyInfo.DeclaringType;
			if (declaringType?.IsNonNullableContext() == true)
			{
				return true;
			}

			return false;
		}

		public static bool IsNonNullable(this PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));

			return propertyInfo.IsNonNullableValueType() || propertyInfo.IsNonNullableReferenceType();
		}

		#endregion

		#region PropertyInfo generic arguments

		public static bool IsGenericArgumentValueType(this PropertyInfo propertyInfo, int index)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(propertyInfo.PropertyType, index, out _);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			return argument.IsValueType;
		}

		public static bool IsGenericArgumentNonNullableValueType(this PropertyInfo propertyInfo, int index)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(propertyInfo.PropertyType, index, out _);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			return argument.IsNonNullableValueType();
		}

		public static bool IsGenericArgumentNonNullableReferenceType(this PropertyInfo propertyInfo, int index)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(propertyInfo.PropertyType, index, out var nullableFlagIndex);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (argument.IsValueType)
			{
				return false;
			}

			// check member
			var flag = GetNullableAttributeFlag(Attribute.GetCustomAttributes(propertyInfo), nullableFlagIndex);
			if (flag != null)
			{
				return flag == 1;
			}

			// check declaring type
			var declaringType = propertyInfo.DeclaringType;
			if (declaringType?.IsNonNullableContext() == true)
			{
				return true;
			}

			return false;
		}

		public static bool IsGenericArgumentNonNullable(this PropertyInfo propertyInfo, int index)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			return propertyInfo.IsGenericArgumentNonNullableValueType(index) || propertyInfo.IsGenericArgumentNonNullableReferenceType(index);
		}

		#endregion

		#region ParameterInfo

		public static bool IsValueType(this ParameterInfo parameterInfo)
		{
			return parameterInfo.ParameterType.IsValueType;
		}

		public static bool IsNonNullableValueType(this ParameterInfo parameterInfo)
		{
			return parameterInfo.ParameterType.IsNonNullableValueType();
		}

		public static bool IsNonNullableReferenceType(this ParameterInfo parameterInfo)
		{
			// value type => not a non nullable reference type
			if (parameterInfo.IsValueType())
			{
				return false;
			}

			// MaybeNullAttribute => not a non nullable reference type
			var isMaybeNull = parameterInfo.CustomAttributes.Any(a => a.AttributeType.FullName == _maybeNullAttributeFullName);
			if (isMaybeNull)
			{
				return false;
			}

			// check parameter
			var flag = GetNullableAttributeFlag(Attribute.GetCustomAttributes(parameterInfo));
			if (flag != null)
			{
				return flag == 1;
			}

			// check method and declaring types
			var method = parameterInfo.Member as MethodInfo;
			if (method?.IsNonNullableContext() == true)
			{
				return true;
			}

			return false;
		}

		public static bool IsNonNullable(this ParameterInfo parameterInfo)
		{
			return parameterInfo.IsNonNullableValueType() || parameterInfo.IsNonNullableReferenceType();
		}

		#endregion

		#region ParameterInfo generic arguments

		public static bool IsGenericArgumentValueType(this ParameterInfo parameterInfo, int index)
		{
			if (parameterInfo == null)
				throw new ArgumentNullException(nameof(parameterInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(parameterInfo.ParameterType, index, out _);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			return argument.IsValueType;
		}

		public static bool IsGenericArgumentNonNullableValueType(this ParameterInfo parameterInfo, int index)
		{
			if (parameterInfo == null)
				throw new ArgumentNullException(nameof(parameterInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(parameterInfo.ParameterType, index, out _);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			return argument.IsNonNullableValueType();
		}

		public static bool IsGenericArgumentNonNullableReferenceType(this ParameterInfo parameterInfo, int index)
		{
			if (parameterInfo == null)
				throw new ArgumentNullException(nameof(parameterInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			var argument = GetGenericArgumentByOrdinal(parameterInfo.ParameterType, index, out var nullableFlagIndex);
			if (argument == null)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (argument.IsValueType)
			{
				return false;
			}

			// check member
			var flag = GetNullableAttributeFlag(Attribute.GetCustomAttributes(parameterInfo), nullableFlagIndex);
			if (flag != null)
			{
				return flag == 1;
			}

			// check method and declaring types
			var method = parameterInfo.Member as MethodInfo;
			if (method?.IsNonNullableContext() == true)
			{
				return true;
			}

			return false;
		}

		public static bool IsGenericArgumentNonNullable(this ParameterInfo parameterInfo, int index)
		{
			if (parameterInfo == null)
				throw new ArgumentNullException(nameof(parameterInfo));
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			return parameterInfo.IsGenericArgumentNonNullableValueType(index) || parameterInfo.IsGenericArgumentNonNullableReferenceType(index);
		}

		#endregion
	}
}
