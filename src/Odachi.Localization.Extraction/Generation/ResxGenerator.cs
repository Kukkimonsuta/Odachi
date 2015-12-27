using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Xml;

namespace Odachi.Localization.Extraction.Generation
{
	public class ResxGenerator : Generator
	{
		public ResxGenerator(Stream stream, Encoding encoding)
		{
			_writer = XmlWriter.Create(stream, new XmlWriterSettings() { Encoding = encoding });
		}

		private XmlWriter _writer;

		private void WriteResHeader(string name, string value)
		{
			_writer.WriteStartElement("root");

			_writer.WriteStartElement("resheader");
			_writer.WriteAttributeString("name", name);

			_writer.WriteStartElement("value");
			_writer.WriteString(value);
			_writer.WriteEndElement();

			_writer.WriteEndElement();
		}

		public override void WriteHeader()
		{
			WriteResHeader("resmimetype", "text/microsoft-resx");
			WriteResHeader("version", "2.0");
			WriteResHeader("reader", "System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			WriteResHeader("writer", "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
		}

		public override void WriteEntry(KeywordMatchGroup match)
		{
			_writer.WriteStartElement("data");
			_writer.WriteAttributeString("name", match.Singular);
			_writer.WriteAttributeString("xml", "space", null, "preserve");

			_writer.WriteStartElement("value");
			_writer.WriteEndElement();

			_writer.WriteEndElement();
		}

		public override void WriteFooter()
		{
			_writer.WriteEndElement();
		}

		public override void Flush()
		{
			_writer.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_writer != null)
				{
					_writer.Flush();
					_writer.Dispose();
					_writer = null;
				}
			}

			base.Dispose(disposing);
		}
	}

	public class ResxGeneratorFactory : IGeneratorFactory
	{
		public string DefaultExtension
		{
			get
			{
				return ".resx";
			}
		}

		public Generator Create(Stream output, Encoding encoding)
		{
			return new ResxGenerator(output, encoding);
		}
	}
}
