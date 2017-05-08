using System;
using System.Collections;
using System.Reflection;

namespace Odachi.AspNetCore.JsonRpc.Model
{
	public class JsonMappedType
	{
		public JsonMappedType(Type netType, JsonValueType jsonType)
		{
			NetType = netType;
			JsonType = jsonType;
		}

		public Type NetType { get; }
		public JsonValueType JsonType { get; }

		public static JsonMappedType FromType(Type type)
		{
			JsonValueType jsonType;

			if (type == typeof(void))
			{
				return null;
			}
			else if (type == typeof(bool) || type == typeof(bool?))
			{
				jsonType = JsonValueType.Boolean;
			}
			else if (
				type == typeof(sbyte) || type == typeof(sbyte?) || type == typeof(byte) || type == typeof(byte?) ||
				type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?) ||
				type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?) ||
				type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?) ||
				type == typeof(float) || type == typeof(double?)
			)
			{
				jsonType = JsonValueType.Number;
			}
			else if (type == typeof(string))
			{
				jsonType = JsonValueType.String;
			}
			else if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				jsonType = JsonValueType.Array;
			}
			else
			{
				jsonType = JsonValueType.Object;
			}

			return new JsonMappedType(type, jsonType);
		}
	}
}
