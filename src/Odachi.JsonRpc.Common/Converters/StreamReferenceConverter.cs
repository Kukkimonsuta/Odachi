using System;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Odachi.Abstractions;

namespace Odachi.JsonRpc.Common.Converters
{
	[Obsolete("Use `Odachi.JsonRpc.Common.Converters.BlobWriteHandler`. Will be removed in next major version.")]
	public class StreamReferenceWriteHandler : IDisposable
	{
		public StreamReferenceWriteHandler(Action<string, IStreamReference> handler)
		{
			if (Current.IsValueCreated && Current.Value != null)
				throw new InvalidOperationException($"Attempt to create nested {nameof(StreamReferenceWriteHandler)}");

			Handler = handler;

			Current.Value = this;
		}

		public Action<string, IStreamReference> Handler { get; }

		public void Dispose()
		{
			if (Current.Value != this)
				throw new InvalidOperationException($"Attempt to dispose wrong instance of {nameof(StreamReferenceWriteHandler)}");

			Current.Value = null;
		}

		#region Static members

		public static ThreadLocal<StreamReferenceWriteHandler> Current = new ThreadLocal<StreamReferenceWriteHandler>();

		#endregion
	}

	[Obsolete("Use `Odachi.JsonRpc.Common.Converters.BlobReadHandler`. Will be removed in next major version.")]
	public class StreamReferenceReadHandler : IDisposable
	{
		public StreamReferenceReadHandler(Func<string, string, IStreamReference> handler)
		{
			if (Current.IsValueCreated && Current.Value != null)
				throw new InvalidOperationException($"Attempt to create nested {nameof(StreamReferenceReadHandler)}");

			Handler = handler;

			Current.Value = this;
		}

		public Func<string, string, IStreamReference> Handler { get; }

		public void Dispose()
		{
			if (Current.Value != this)
				throw new InvalidOperationException($"Attempt to dispose wrong instance of {nameof(StreamReferenceReadHandler)}");

			Current.Value = null;
		}

		#region Static members

		public static ThreadLocal<StreamReferenceReadHandler> Current = new ThreadLocal<StreamReferenceReadHandler>();

		#endregion
	}

	[Obsolete("Use `Odachi.JsonRpc.Common.Converters.BlobConverter`. Will be removed in next major version.")]
	public class StreamReferenceConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(IStreamReference).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var name = (string)serializer.Deserialize(reader, typeof(string));

			if (StreamReferenceReadHandler.Current.Value == null)
				throw new NotSupportedException("Deserializing stream references is not supported by current consumer.");

			return StreamReferenceReadHandler.Current.Value.Handler(reader.Path, name);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var reference = (IStreamReference)value;

			writer.WriteValue(writer.Path);

			if (StreamReferenceWriteHandler.Current.Value == null)
				throw new NotSupportedException("Serializing stream references is not supported by current consumer.");

			StreamReferenceWriteHandler.Current.Value.Handler(writer.Path, reference);
		}
	}
}
