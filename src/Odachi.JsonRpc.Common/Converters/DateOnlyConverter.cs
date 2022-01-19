using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Odachi.JsonRpc.Common.Converters
{
	public class DateOnlyConverter : JsonConverter<DateOnly>
	{
		private const string DateFormat = "yyyy-MM-dd";

		public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return DateOnly.ParseExact((string)reader.Value, DateFormat, CultureInfo.InvariantCulture);
		}

		public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
		}
	}
}
