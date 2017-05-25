using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Odachi.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Converters
{
	public class StreamReferenceConverter : JsonConverter
	{
		public StreamReferenceConverter(IHttpContextAccessor httpContextAccessor)
		{
			if (httpContextAccessor == null)
				throw new ArgumentNullException(nameof(httpContextAccessor));

			_httpContextAccessor = httpContextAccessor;
		}

		private IHttpContextAccessor _httpContextAccessor;

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(IStreamReference);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var fieldName = (string)serializer.Deserialize(reader, typeof(string));

			var form = _httpContextAccessor.HttpContext.Request?.Form;
			if (form == null)
				return null;

			var file = form.Files[fieldName];
			if (file == null)
				return null;

			return new FormFileStreamReference(file);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}

	public class FormFileStreamReference : IStreamReference
	{
		public FormFileStreamReference(IFormFile file)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));

			_file = file;
		}

		private IFormFile _file;

		public string Name => _file.FileName;

		public Stream OpenReadStream() => _file.OpenReadStream();
	}
}
