using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Odachi.Abstractions;
using Odachi.Extensions.Primitives;

namespace Odachi.JsonRpc.Common.Converters
{
	[Obsolete("Entity reference concept is obsolete, see `Odachi.Abstractions.IEntityReference`. Will be removed in next major version.")]
	public class EntityReferenceConverter : JsonConverter
	{
		public EntityReferenceConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			var info = objectType.GetTypeInfo();

			return objectType == typeof(IEntityReference);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var transport = serializer.Deserialize<Transport>(reader);

			return new EntityReference(transport.Id);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			// todo: for now serialize whole object, but explore how we could avoid it and
			// pre-fill these in some other manner that will be natural to .NET clients as well as TypeScript

			serializer.Serialize(writer, value);
		}

		#region Nested type: Transport

		public class Transport
		{
			public int Id { get; set; }
		}

		#endregion
	}
}
