using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using MimeKit.Utils;
using MimeKit;

namespace Odachi.Mail
{
	public class MailTemplateContext
	{
		public string Subject { get; set; }

		public IList<MimePart> BodyParts { get; } = new List<MimePart>();
	}

	public abstract class MailTemplateBase
	{
		public MailTemplateLayout Layout { get; set; }

		protected string Subject
		{
			get => Context.Subject;
			set => Context.Subject = value;
		}

		internal MailTemplateContext Context { get; set; }
		internal IDictionary<string, Func<TextWriter, Task>> Sections { get; set; }

		#region Razor (mostly butchered from https://github.com/aspnet/Mvc/blob/rel/1.1.2/src/Microsoft.AspNetCore.Mvc.Razor/RazorPage.cs)

		// TODO: Needs to be updated to 2.0.0

		private AttributeInfo _attributeInfo;

		public TextWriter Output { get; set; }
		public abstract Task ExecuteAsync();

		protected void DefineSection(string name, Func<TextWriter, Task> section)
		{
			if (Sections == null)
				Sections = new Dictionary<string, Func<TextWriter, Task>>();

			Sections[name] = section;
		}

		protected void Write(object value)
		{
			if (Output == null)
				throw new InvalidOperationException("No output writer defined");

			if (value == null)
				return;

			WriteTo(Output, value.ToString());
		}
		protected void Write(string value)
		{
			if (Output == null)
				throw new InvalidOperationException("No output writer defined");

			if (value == null)
				return;

			WriteTo(Output, value);
		}

		protected void WriteTo(TextWriter writer, object value)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (value == null)
				return;

			WriteTo(writer, value.ToString());
		}
		protected void WriteTo(TextWriter writer, string value)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (string.IsNullOrEmpty(value))
				return;

			HtmlEncoder.Default.Encode(Output, value);
		}

		protected void WriteLiteral(object value)
		{
			if (Output == null)
				throw new InvalidOperationException("No output writer defined");

			if (value == null)
				return;

			WriteLiteralTo(Output, value.ToString());
		}
		protected void WriteLiteral(string value)
		{
			if (Output == null)
				throw new InvalidOperationException("No output writer defined");

			if (value == null)
				return;

			WriteLiteralTo(Output, value);
		}

		protected void WriteLiteralTo(TextWriter writer, object value)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (value == null)
				return;

			WriteLiteralTo(writer, value.ToString());
		}
		protected void WriteLiteralTo(TextWriter writer, string value)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (string.IsNullOrEmpty(value))
				return;

			writer.Write(value);
		}

		public virtual void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
		{
			BeginWriteAttributeTo(Output, name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount);
		}

		public virtual void BeginWriteAttributeTo(TextWriter writer, string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));
			if (prefix == null)
				throw new ArgumentNullException(nameof(prefix));
			if (suffix == null)
				throw new ArgumentNullException(nameof(suffix));

			_attributeInfo = new AttributeInfo(name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount);

			// Single valued attributes might be omitted in entirety if it the attribute value strictly evaluates to
			// null  or false. Consequently defer the prefix generation until we encounter the attribute value.
			if (attributeValuesCount != 1)
			{
				WritePositionTaggedLiteral(writer, prefix, prefixOffset);
			}
		}

		public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
		{
			WriteAttributeValueTo(Output, prefix, prefixOffset, value, valueOffset, valueLength, isLiteral);
		}

		public void WriteAttributeValueTo(TextWriter writer, string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
		{
			if (_attributeInfo.AttributeValuesCount == 1)
			{
				if (string.IsNullOrEmpty(prefix) && (value == null || (value is bool && !(bool)value)))
				{
					// Value is either null or the bool 'false' with no prefix; don't render the attribute.
					_attributeInfo.Suppressed = true;
					return;
				}

				// We are not omitting the attribute. Write the prefix.
				WritePositionTaggedLiteral(writer, _attributeInfo.Prefix, _attributeInfo.PrefixOffset);

				if (string.IsNullOrEmpty(prefix) && (value is bool && (bool)value))
				{
					// The value is just the bool 'true', write the attribute name instead of the string 'True'.
					value = _attributeInfo.Name;
				}
			}

			// This block handles two cases.
			// 1. Single value with prefix.
			// 2. Multiple values with or without prefix.
			if (value != null)
			{
				if (!string.IsNullOrEmpty(prefix))
				{
					WritePositionTaggedLiteral(writer, prefix, prefixOffset);
				}

				WriteUnprefixedAttributeValueTo(writer, value, isLiteral);
			}
		}

		public virtual void EndWriteAttribute()
		{
			EndWriteAttributeTo(Output);
		}

		public virtual void EndWriteAttributeTo(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (!_attributeInfo.Suppressed)
			{
				WritePositionTaggedLiteral(writer, _attributeInfo.Suffix, _attributeInfo.SuffixOffset);
			}
		}

		private void WriteUnprefixedAttributeValueTo(TextWriter writer, object value, bool isLiteral)
		{
			var stringValue = value as string;

			// The extra branching here is to ensure that we call the Write*To(string) overload where possible.
			if (isLiteral && stringValue != null)
			{
				WriteLiteralTo(writer, stringValue);
			}
			else if (isLiteral)
			{
				WriteLiteralTo(writer, value);
			}
			else if (stringValue != null)
			{
				WriteTo(writer, stringValue);
			}
			else
			{
				WriteTo(writer, value);
			}
		}

		private void WritePositionTaggedLiteral(TextWriter writer, string value, int position)
		{
			WriteLiteralTo(writer, value);
		}

		#endregion

		#region Mail helpers

		protected string Embed(string contentType, string manifestResourceName, string fileName = null)
		{
			var assembly = GetType().GetTypeInfo().Assembly;

			var stream = assembly.GetManifestResourceStream(manifestResourceName);
			if (stream == null)
				throw new FileNotFoundException($"File '{manifestResourceName}' not found in assembly '{assembly.FullName}'");
			try
			{
				return Embed(contentType, stream, fileName);
			}
			catch (Exception)
			{
				stream.Dispose();
				throw;
			}
		}
		protected string Embed(string contentType, Stream stream, string fileName = null)
		{
			var contentId = MimeUtils.GenerateMessageId();

			Context.BodyParts.Add(new MimePart(contentType)
			{
				ContentId = contentId,
				ContentObject = new ContentObject(stream, ContentEncoding.Default),
				ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
				ContentTransferEncoding = ContentEncoding.Base64,
				FileName = fileName,
			});

			return contentId;
		}

		protected void Attach(string contentType, string manifestResourceName, string fileName = null)
		{
			var assembly = GetType().GetTypeInfo().Assembly;

			var stream = assembly.GetManifestResourceStream(manifestResourceName);
			if (stream == null)
				throw new FileNotFoundException($"File '{manifestResourceName}' not found in assembly '{assembly.FullName}'");
			try
			{
				Attach(contentType, stream, fileName);
			}
			catch (Exception)
			{
				stream.Dispose();
				throw;
			}
		}
		protected void Attach(string contentType, Stream stream, string fileName = null)
		{
			Context.BodyParts.Add(new MimePart(contentType)
			{
				ContentObject = new ContentObject(stream, ContentEncoding.Default),
				ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
				ContentTransferEncoding = ContentEncoding.Base64,
				FileName = fileName,
			});
		}

		#endregion

		#region Nested type: AttributeInfo

		private struct AttributeInfo
		{
			public AttributeInfo(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
			{
				Name = name;
				Prefix = prefix;
				PrefixOffset = prefixOffset;
				Suffix = suffix;
				SuffixOffset = suffixOffset;
				AttributeValuesCount = attributeValuesCount;

				Suppressed = false;
			}

			public int AttributeValuesCount { get; }

			public string Name { get; }

			public string Prefix { get; }

			public int PrefixOffset { get; }

			public string Suffix { get; }

			public int SuffixOffset { get; }

			public bool Suppressed { get; set; }
		}

		#endregion
	}
}
