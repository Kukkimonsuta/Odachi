using System;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Odachi.Abstractions;

namespace Odachi.JsonRpc.Common.Converters
{
	public class BlobWriteHandler : IDisposable
	{
		public BlobWriteHandler(Action<string, IBlob> handler)
		{
			if (Current.Value != null)
				throw new InvalidOperationException($"Attempt to create nested {nameof(BlobWriteHandler)}");

			Handler = handler;

			Current.Value = this;
		}

		public Action<string, IBlob> Handler { get; }

		public void Dispose()
		{
			if (Current.Value != this)
				throw new InvalidOperationException($"Attempt to dispose wrong instance of {nameof(BlobWriteHandler)}");

			Current.Value = null;
		}

		#region Static members

		public static AsyncLocal<BlobWriteHandler> Current = new AsyncLocal<BlobWriteHandler>();

		#endregion
	}

	public class BlobReadHandler : IDisposable
	{
		public BlobReadHandler(Func<string, string, IBlob> handler)
		{
			if (Current.Value != null)
				throw new InvalidOperationException($"Attempt to create nested {nameof(BlobReadHandler)}");

			Handler = handler;

			Current.Value = this;
		}

		public Func<string, string, IBlob> Handler { get; }

		public void Dispose()
		{
			if (Current.Value != this)
				throw new InvalidOperationException($"Attempt to dispose wrong instance of {nameof(BlobReadHandler)}");

			Current.Value = null;
		}

		#region Static members

		public static AsyncLocal<BlobReadHandler> Current = new AsyncLocal<BlobReadHandler>();

		#endregion
	}

	public class BlobConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(IBlob).IsAssignableFrom(objectType.GetTypeInfo());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var name = (string)serializer.Deserialize(reader, typeof(string));

			if (BlobReadHandler.Current.Value == null)
				throw new NotSupportedException("Deserializing blobs is not supported by current consumer.");

			return BlobReadHandler.Current.Value.Handler(reader.Path, name);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var blob = (IBlob)value;

			writer.WriteValue(writer.Path);

			if (BlobWriteHandler.Current.Value == null)
				throw new NotSupportedException("Serializing blobs is not supported by current consumer.");

			BlobWriteHandler.Current.Value.Handler(writer.Path, blob);
		}
	}
}
