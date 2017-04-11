using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.Encodings.Web;

namespace Odachi.Mail
{
	public abstract class MailTemplateBase
	{
		public MailTemplateLayout Layout { get; set; }

		internal IDictionary<string, Func<TextWriter, Task>> Sections { get; set; }

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
			if (value == null)
				return;

			Write(value.ToString());
		}
		protected void Write(string value)
		{
			if (Output == null)
				throw new InvalidOperationException("No output writer defined");

			WriteTo(Output, value);
		}

		protected void WriteLiteral(string value)
		{
			if (Output == null)
				throw new InvalidOperationException("No output writer defined");

			WriteLiteralTo(Output, value);
		}

		protected void WriteTo(TextWriter writer, string value)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (string.IsNullOrEmpty(value))
				return;

			HtmlEncoder.Default.Encode(Output, value);
		}

		protected void WriteLiteralTo(TextWriter writer, string value)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (string.IsNullOrEmpty(value))
				return;

			writer.Write(value);
		}
	}
}
