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
			if (Attribute.GetCustomAttributes(fieldInfo).FirstOrDefault(a => a.GetType().FullName == _nullableAttributeFullName) is Attribute attribute)
			{
				var attributeType = attribute.GetType();
				var flagsField = attributeType.GetField("NullableFlags");

				if (flagsField?.GetValue(attribute) is byte[] flags)
				{
					return flags.FirstOrDefault() == 1;
				}
			}

			// check declaring type
			var declaringType = fieldInfo.DeclaringType;
			if (declaringType != null && declaringType.IsNonNullableContext() == true)
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

		#region PropertyInfo

		public static bool IsValueType(this PropertyInfo propertyInfo)
		{
			return propertyInfo.PropertyType.IsValueType;
		}

		public static bool IsNonNullableValueType(this PropertyInfo propertyInfo)
		{
			return propertyInfo.PropertyType.IsNonNullableValueType();
		}

		public static bool IsNonNullableReferenceType(this PropertyInfo propertyInfo)
		{
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
			if (Attribute.GetCustomAttributes(propertyInfo).FirstOrDefault(a => a.GetType().FullName == _nullableAttributeFullName) is Attribute attribute)
			{
				var attributeType = attribute.GetType();
				var flagsField = attributeType.GetField("NullableFlags");

				if (flagsField?.GetValue(attribute) is byte[] flags)
				{
					return flags.FirstOrDefault() == 1;
				}
			}

			// check declaring type
			var declaringType = propertyInfo.DeclaringType;
			if (declaringType != null && declaringType.IsNonNullableContext() == true)
			{
				return true;
			}

			return false;
		}

		public static bool IsNonNullable(this PropertyInfo propertyInfo)
		{
			return propertyInfo.IsNonNullableValueType() || propertyInfo.IsNonNullableReferenceType();
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
			if (Attribute.GetCustomAttributes(parameterInfo).FirstOrDefault(a => a.GetType().FullName == _nullableAttributeFullName) is Attribute attribute)
			{
				var attributeType = attribute.GetType();
				var flagsField = attributeType.GetField("NullableFlags");

				if (flagsField?.GetValue(attribute) is byte[] flags)
				{
					return flags.FirstOrDefault() == 1;
				}
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
	}
}
