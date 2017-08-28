using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Odachi.Extensions.Collections;

namespace Odachi.JsonRpc.Common.Converters
{
	public class PageConverter : JsonConverter
	{
		public PageConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(Page).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Type type = objectType;
			Type itemType = null;
			while (type != null && type != typeof(Page))
			{
				var typeInfo = type.GetTypeInfo();

				if (!typeInfo.IsGenericType)
				{
					type = typeInfo.BaseType;
					continue;
				}

				if (typeInfo.GetGenericTypeDefinition() != typeof(Page<>))
				{
					type = typeInfo.BaseType;
					continue;
				}

				itemType = type.GenericTypeArguments[0];
				break;
			}

			if (type == null)
			{
				throw new InvalidOperationException("Cannot deseralize non-generic page");
			}

			var transportType = typeof(Transport<>).MakeGenericType(itemType);
			var transport = (TransportBase)serializer.Deserialize(reader, transportType);

			return Activator.CreateInstance(objectType, transport.GetData(), transport.Number, transport.Size, transport.Total, transport.Overflow);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var page = (Page)value;

			var transport = new Transport()
			{
				Data = page.Data,
				Number = page.Number,
				Size = page.Size,
				Count = page.Count,
				Total = page.Total,
				Overflow = page.Overflow,
			};

			serializer.Serialize(writer, transport);
		}

		#region Nested type: TransportBase

		public abstract class TransportBase
		{
			public int Number { get; set; }
			public int Size { get; set; }
			public int Count { get; set; }
			public int? Total { get; set; }
			public bool Overflow { get; set; }

			public abstract IEnumerable GetData();
		}

		#endregion

		#region Nested type: Transport

		public class Transport : TransportBase
		{
			public IEnumerable Data { get; set; }

			public override IEnumerable GetData() => Data;
		}

		#endregion

		#region Nested type: Transport<T>

		public class Transport<T> : TransportBase
		{
			public IEnumerable<T> Data { get; set; }

			public override IEnumerable GetData() => Data;
		}

		#endregion
	}
}
