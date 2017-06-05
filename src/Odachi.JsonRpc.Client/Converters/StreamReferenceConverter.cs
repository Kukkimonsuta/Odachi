using System;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Odachi.Abstractions;

namespace Odachi.AspNetCore.JsonRpc.Converters
{
	public class StreamReferenceHandler : IDisposable
	{
		public StreamReferenceHandler(Action<string, IStreamReference> handler)
		{
			if (Current.IsValueCreated && Current.Value != null)
				throw new InvalidOperationException("Attempt to create nested StreamReferenceHandler");

			Handler = handler;

			Current.Value = this;
		}

		public Action<string, IStreamReference> Handler { get; }

		public void Dispose()
		{
			if (Current.Value != this)
				throw new InvalidOperationException("Attempt to dispose wrong instance of StreamReferenceHandler");

			Current.Value = null;
		}

		#region Static members

		public static ThreadLocal<StreamReferenceHandler> Current = new ThreadLocal<StreamReferenceHandler>();

		#endregion
	}

	public class StreamReferenceConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(IStreamReference).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var reference = (IStreamReference)value;

			writer.WriteValue(writer.Path);

			StreamReferenceHandler.Current.Value.Handler(writer.Path, reference);
		}
	}
}
