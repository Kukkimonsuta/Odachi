using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Odachi.JsonRpc.Common.Converters
{
	public class TimeOnlyConverter : JsonConverter<TimeOnly>
	{
		private const string TimeFormat = "HH:mm:ss.FFFFFFF";

		public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return TimeOnly.ParseExact((string)reader.Value, TimeFormat, CultureInfo.InvariantCulture);
		}

		public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString(TimeFormat, CultureInfo.InvariantCulture));
		}
	}
}
