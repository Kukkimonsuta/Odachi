using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace Odachi.Localization.Extraction.Generation
{
	public class PotGenerator : Generator
	{
		public PotGenerator(Stream stream, Encoding encoding)
		{
			_writer = new StreamWriter(stream, encoding);
		}

		private TextWriter _writer;

		public override void WriteHeader()
		{
			_writer.WriteLine(@"# Copyright (C) YEAR THE PACKAGE'S COPYRIGHT HOLDER
# This file is distributed under the same license as the PACKAGE package.
# FIRST AUTHOR <EMAIL@ADDRESS>, YEAR.
#
#, fuzzy
msgid """"
msgstr """"
""Project-Id-Version: PACKAGE VERSION\n""
""Report-Msgid-Bugs-To: MAINTAINER <EMAIL@ADRESS>\n""");

			_writer.WriteLine(@"""POT-Creation-Date: {0}\n""", DateTime.Now.ToString("yyyy-MM-dd HH:mmK"));

			_writer.WriteLine(@"""PO-Revision-Date: YEAR-MO-DA HO:MI+ZONE\n""
""Last-Translator: FULL NAME <EMAIL@ADDRESS>\n""
""Language-Team: LANGUAGE <LL@li.org>\n""
""Language: \n""
""MIME-Version: 1.0\n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""
");
		}

		public override void WriteEntry(KeywordMatchGroup match)
		{
			foreach (var source in match.Sources)
			{
				_writer.WriteLine("#: {0}:{1}", source.FileName, source.LineIndex + 1);
			}

			if (match.Context != null)
				_writer.WriteLine("msgctxt \"{0}\"", match.Context);
			_writer.WriteLine("msgid \"{0}\"", match.Singular);
			if (match.Plural != null)
			{
				_writer.WriteLine("msgid_plural \"{0}\"", match.Plural);
				_writer.WriteLine("msgstr[0] \"\"");
				_writer.WriteLine("msgstr[1] \"\"");
			}
			else
			{
				_writer.WriteLine("msgstr \"\"");
			}
			_writer.WriteLine();
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
					_writer.Dispose();
					_writer = null;
				}
			}

			base.Dispose(disposing);
		}
	}

	public class PotGeneratorFactory : IGeneratorFactory
	{
		public string DefaultExtension
		{
			get
			{
				return ".pot";
			}
		}

		public Generator Create(Stream output, Encoding encoding)
		{
			return new PotGenerator(output, encoding);
		}
	}
}
