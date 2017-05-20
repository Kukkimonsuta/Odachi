using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeGen.IO
{
	public class IndentWriter : TextWriter
	{
		public IndentWriter(TextWriter writer)
			: this(writer, null)
		{
		}
		public IndentWriter(TextWriter writer, IFormatProvider formatProvider)
			: base(formatProvider)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			Writer = writer;
		}

		private int _indentLevel;

		protected TextWriter Writer { get; }
		protected int IndentLevel { get { return _indentLevel; } }

		public string IndentString { get; set; } = "\t";

		public void Indent(int magnitude)
		{
			_indentLevel = Math.Max(0, IndentLevel + magnitude);
		}

		public void WriteIndent()
		{
			for (var i = 0; i < IndentLevel; i++)
				Writer.Write(IndentString);
		}

		public void WriteIndented(string format, params object[] args)
		{
			WriteIndented(string.Format(format, args));
		}
		public void WriteIndented(string text)
		{
			foreach (var line in text.Replace("\r\n", "\n").Split(new[] { '\n' }, StringSplitOptions.None))
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

		public IDisposable WriteIndentedBlock(string prefix = "", string open = "{", string close = "}", string suffix = "")
		{
			return new Block(this, prefix, open, close, suffix, false);
		}

		public IDisposable WriteBlock(string prefix = "", string open = "{", string close = "}", string suffix = "")
		{
			return new Block(this, prefix, open, close, suffix, true);
		}

		#region TextWriter

		public override string NewLine
		{
			get { return Writer.NewLine; }
			set { Writer.NewLine = value; }
		}

		public override IFormatProvider FormatProvider => Writer.FormatProvider;

		public override Encoding Encoding => Writer.Encoding;

		public override void Write(bool value)
		{
			Writer.Write(value);
		}

		public override void Write(char value)
		{
			Writer.Write(value);
		}

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

#if !DOTNET
		public override void Close() => Writer.Close();
#endif

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
			public Block(IndentWriter writer, string prefix,  string open, string close, string suffix, bool inline)
			{
				if (!inline)
					writer.WriteIndent();
				if (!string.IsNullOrEmpty(prefix))
					writer.Write(prefix);
				if (!string.IsNullOrEmpty(open))
					writer.Write(open);
				writer.WriteLine();

				writer.Indent(1);

				_writer = writer;
				_close = close;
				_suffix = suffix;
				_inline = inline;
			}

			private IndentWriter _writer;
			private string _close;
			private string _suffix;
			private bool _inline;

			public void Dispose()
			{
				_writer.Indent(-1);

				_writer.WriteIndent();
				if (!string.IsNullOrEmpty(_close))
					_writer.Write(_close);
				if (!string.IsNullOrEmpty(_suffix))
					_writer.Write(_suffix);

				if (!_inline)
					_writer.WriteLine();
			}
		}

		#endregion
	}
}
