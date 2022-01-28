#nullable enable

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Odachi.JsonRpc.Common.Converters
{
	public class NullableTimeOnlyConverter : JsonConverter<TimeOnly?>
	{
		private const string TimeFormat = "HH:mm:ss.FFFFFFF";

		public override TimeOnly? ReadJson(JsonReader reader, Type objectType, TimeOnly? existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var stringValue = (string?)reader.Value;

			if (stringValue == null || stringValue.Length <= 0)
			{
				return null;
			}
			else
			{
				return TimeOnly.ParseExact(stringValue, TimeFormat, CultureInfo.InvariantCulture);
			}
		}

		public override void WriteJson(JsonWriter writer, TimeOnly? value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(value?.ToString(TimeFormat, CultureInfo.InvariantCulture));
			}
		}
	}
}
