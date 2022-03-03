#nullable enable

using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Xml;

namespace Odachi.JsonRpc.Common.Converters
{
	public class NullableTimeSpanConverter : JsonConverter<TimeSpan?>
	{
		public override TimeSpan? ReadJson(JsonReader reader, Type objectType, TimeSpan? existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var stringValue = (string?)reader.Value;

			if (stringValue == null || stringValue.Length <= 0)
			{
				return null;
			}
			else
			{
				return XmlConvert.ToTimeSpan(stringValue);
			}
		}

		public override void WriteJson(JsonWriter writer, TimeSpan? value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(XmlConvert.ToString(value.Value));
			}
		}
	}
}
