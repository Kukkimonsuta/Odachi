#nullable enable

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Odachi.JsonRpc.Common.Converters
{
	public class NullableDateOnlyConverter : JsonConverter<DateOnly?>
	{
		private const string DateFormat = "yyyy-MM-dd";

		public override DateOnly? ReadJson(JsonReader reader, Type objectType, DateOnly? existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var stringValue = (string?)reader.Value;

			if (stringValue == null || stringValue.Length <= 0)
			{
				return null;
			}
			else
			{
				return DateOnly.ParseExact(stringValue, DateFormat, CultureInfo.InvariantCulture);
			}
		}

		public override void WriteJson(JsonWriter writer, DateOnly? value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(value.Value.ToString(DateFormat, CultureInfo.InvariantCulture));
			}
		}
	}
}
