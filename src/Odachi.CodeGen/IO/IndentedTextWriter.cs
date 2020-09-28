using Odachi.Extensions.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeGen.IO
{
	public class IndentedTextWriter : TextWriter
	{
		public IndentedTextWriter(TextWriter writer)
			: this(writer, null)
		{
		}
		public IndentedTextWriter(TextWriter writer, IFormatProvider formatProvider)
			: base(formatProvider)
		{
			Writer = writer ?? throw new ArgumentNullException(nameof(writer));
		}

		protected TextWriter Writer { get; }
		protected int IndentLevel { get; private set; }
		protected bool ShouldWriteSeparatingLine { get; set; }

		/// <summary>
		/// String used for single indent level.
		/// </summary>
		public string IndentString { get; set; } = "\t";
		/// <summary>
		/// Insert new line + indent between blocks prefix and open brace.
		/// </summary>
		public bool OpenBlockOnNewLine { get; set; } = false;

		protected void WriteSeparatingLineIfRequested()
		{
			if (ShouldWriteSeparatingLine)
			{
				Writer.WriteLine();
				ShouldWriteSeparatingLine = false;
			}
		}

		/// <summary>
		/// Changes indentation level by magnitude.
		/// </summary>
		public void Indent(int magnitude)
		{
			IndentLevel = Math.Max(0, IndentLevel + magnitude);
		}

		/// <summary>
		/// Write empty line separating content blocks. Line is ignored before block ends.
		/// </summary>
		public void WriteSeparatingLine()
		{
			ShouldWriteSeparatingLine = true;
		}

		/// <summary>
		/// Writes indent to underlying writer. Assumes position is at start of a line.
		/// </summary>
		public void WriteIndent()
		{
			WriteSeparatingLineIfRequested();

			for (var i = 0; i < IndentLevel; i++)
			{
				Writer.Write(IndentString);
			}
		}

		/// <summary>
		/// Writes indented text to underlying writer. Embedded new lines are indented.
		/// </summary>
		public void WriteIndentedLine(string format, params object[] args)
		{
			WriteIndentedLine(string.Format(format, args));
		}
		/// <summary>
		/// Writes indented text to underlying writer. Embedded new lines are indented.
		/// </summary>
		public void WriteIndentedLine(string text)
		{
			WriteSeparatingLineIfRequested();

			foreach (var line in text.GetLines())
			{
				if (string.IsNullOrWhiteSpace(line))
				{
					WriteLine();
					continue;
				}

				WriteIndent();
				WriteLine(line);
			}
		}

		/// <summary>
		/// Writes block to underlying writer. Opening of the block is indented and new line is inserted after suffix.
		/// </summary>
		public IDisposable WriteIndentedBlock(string prefix = "", string open = "{", string close = "}", string suffix = "", bool writeSeparatingLine = true)
		{
			return new Block(this, prefix, open, close, suffix, false, writeSeparatingLine);
		}

		/// <summary>
		/// Writes block to underlying writer. Opening of the block is not indented and new line is not inserted after suffix.
		/// </summary>
		public IDisposable WriteBlock(string prefix = "", string open = "{", string close = "}", string suffix = "")
		{
			return new Block(this, prefix, open, close, suffix, true, false);
		}

		#region TextWriter

		public override string NewLine
		{
			get { return Writer.NewLine; }
			set { Writer.NewLine = value; }
		}

		public override IFormatProvider FormatProvider => Writer.FormatProvider;

		public override Encoding Encoding => Writer.Encoding;

		public override void Write(bool value) => Writer.Write(value);

		public override void Write(char value) => Writer.Write(value);

		public override void Write(char[] buffer) => Writer.Write(buffer);

		public override void Write(char[] buffer, int index, int count) => Writer.Write(buffer, index, count);

		public override void Write(decimal value) => Writer.Write(value);

		public override void Write(double value) => Writer.Write(value);

		public override void Write(float value) => Writer.Write(value);

		public override void Write(int value) => Writer.Write(value);

		public override void Write(long value) => Writer.Write(value);

		public override void Write(object value) => Writer.Write(value);

		public override void Write(string format, object arg0) => Writer.Write(format, arg0);

		public override void Write(string format, object arg0, object arg1) => Writer.Write(format, arg0, arg1);

		public override void Write(string format, object arg0, object arg1, object arg2) => Writer.Write(format, arg0, arg1, arg2);

		public override void Write(string format, params object[] arg) => Writer.Write(format, arg);

		public override void Write(string value) => Writer.Write(value);

		public override void Write(uint value) => Writer.Write(value);

		public override void Write(ulong value) => Writer.Write(value);

		public override Task WriteAsync(char value) => Writer.WriteAsync(value);

		public override Task WriteAsync(char[] buffer, int index, int count) => Writer.WriteAsync(buffer, index, count);

		public override Task WriteAsync(string value) => Writer.WriteAsync(value);

		public override void WriteLine() => Writer.WriteLine();

		public override void WriteLine(bool value) => Writer.WriteLine(value);

		public override void WriteLine(char value) => Writer.WriteLine(value);

		public override void WriteLine(char[] buffer) => Writer.WriteLine(buffer);

		public override void WriteLine(char[] buffer, int index, int count) => Writer.WriteLine(buffer, index, count);

		public override void WriteLine(decimal value) => Writer.WriteLine(value);

		public override void WriteLine(double value) => Writer.WriteLine(value);

		public override void WriteLine(float value) => Writer.WriteLine(value);

		public override void WriteLine(int value) => Writer.WriteLine(value);

		public override void WriteLine(long value) => Writer.WriteLine(value);

		public override void WriteLine(object value) => Writer.WriteLine(value);

		public override void WriteLine(string format, object arg0) => Writer.WriteLine(format, arg0);

		public override void WriteLine(string format, object arg0, object arg1) => Writer.WriteLine(format, arg0, arg1);

		public override void WriteLine(string format, object arg0, object arg1, object arg2) => Writer.WriteLine(format, arg0, arg1, arg2);

		public override void WriteLine(string format, params object[] arg) => Writer.WriteLine(format, arg);

		public override void WriteLine(string value) => Writer.WriteLine(value);

		public override void WriteLine(uint value) => Writer.WriteLine(value);

		public override void WriteLine(ulong value) => Writer.WriteLine(value);

		public override Task WriteLineAsync() => Writer.WriteLineAsync();

		public override Task WriteLineAsync(char value) => Writer.WriteLineAsync(value);

		public override Task WriteLineAsync(char[] buffer, int index, int count) => Writer.WriteLineAsync(buffer, index, count);

		public override Task WriteLineAsync(string value) => Writer.WriteLineAsync(value);

		public override void Flush() => Writer.Flush();

		public override Task FlushAsync() => Writer.FlushAsync();

		public override void Close() => Writer.Close();

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Writer != null)
				{
					Writer.Dispose();
				}
			}
		}

		#endregion

		#region Nested type: Indent block

		public sealed class Block : IDisposable
		{
			public Block(IndentedTextWriter writer, string prefix, string open, string close, string suffix, bool inline, bool writeSeparatingLine)
			{
				if (writer.OpenBlockOnNewLine)
				{
					if (!string.IsNullOrEmpty(prefix))
					{
						if (!inline)
							writer.WriteIndent();

						writer.Write(prefix.TrimEnd());
					}
					writer.WriteLine();
					writer.WriteIndent();
				}
				else
				{
					if (!inline)
						writer.WriteIndent();
					if (!string.IsNullOrEmpty(prefix))
						writer.Write(prefix);
				}
				if (!string.IsNullOrEmpty(open))
					writer.Write(open);
				writer.WriteLine();

				writer.Indent(1);

				_writer = writer;
				_close = close;
				_suffix = suffix;
				_inline = inline;
				_writeSeparatingLine = writeSeparatingLine;
			}

			private IndentedTextWriter _writer;
			private string _close;
			private string _suffix;
			private bool _inline;
			private bool _writeSeparatingLine;

			public void Dispose()
			{
				if (_writer.ShouldWriteSeparatingLine)
					_writer.ShouldWriteSeparatingLine = false;

				_writer.Indent(-1);

				_writer.WriteIndent();
				if (!string.IsNullOrEmpty(_close))
					_writer.Write(_close);
				if (!string.IsNullOrEmpty(_suffix))
					_writer.Write(_suffix);

				if (!_inline)
					_writer.WriteLine();

				if (_writeSeparatingLine)
					_writer.WriteSeparatingLine();
			}
		}

		#endregion
	}
}
