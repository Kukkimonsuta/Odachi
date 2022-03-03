using Newtonsoft.Json;
using System;
using System.Xml;

namespace Odachi.JsonRpc.Common.Converters
{
	public class TimeSpanConverter : JsonConverter<TimeSpan>
	{
		public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return XmlConvert.ToTimeSpan((string)reader.Value);
		}

		public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
		{
			writer.WriteValue(XmlConvert.ToString(value));
		}
	}
}
