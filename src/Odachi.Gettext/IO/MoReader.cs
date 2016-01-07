using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Gettext.IO
{
	public class MoReader : IDisposable, IGettextReader
	{
		private static readonly byte[] MAGIC_BE = new byte[] { 0x95, 0x04, 0x12, 0xde };
		private static readonly byte[] MAGIC_LE = new byte[] { 0xde, 0x12, 0x04, 0x95 };
		private static readonly char CONTEXT_SEPARATOR = '\u0004';
		private static readonly char[] CONTEXT_SEPARATORS = new[] { CONTEXT_SEPARATOR };
		private static readonly char STRING_SEPARATOR = '\0';
		private static readonly char[] STRING_SEPARATORS = new[] { STRING_SEPARATOR };

		public MoReader(string fileName)
			: this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
		}
		public MoReader(Stream stream, bool leaveOpen = false)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			_stream = stream;
			_leaveOpen = leaveOpen;

			_encoding = Encoding.UTF8;

			var endianness = ReadMagic();

			_reader = new BinaryReader(stream, endianness: endianness, leaveOpen: leaveOpen);

			Version = ReadVersion();

			StringCount = _reader.ReadInt32();
			OriginalTableOffset = _reader.ReadInt32();
			TranslationTableOffset = _reader.ReadInt32();
		}

		private Stream _stream;
		private bool _leaveOpen;

		private BinaryReader _reader;
		private int _index = -1;
		private Encoding _encoding;

		public Version Version { get; }

		public Encoding Encoding
		{
			get { return _encoding; }
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				_encoding = value;
			}
		}

		public int StringCount { get; }
		public int OriginalTableOffset { get; }
		public int TranslationTableOffset { get; }

		private Endianness ReadMagic()
		{
			var buffer = new byte[4];

			_stream.Read(buffer, 0, 4);

			if (buffer.SequenceEqual(MAGIC_LE))
				return Endianness.Little;
			else if (buffer.SequenceEqual(MAGIC_BE))
				return Endianness.Big;
			else
				throw new InvalidOperationException("Invalid file format.");
		}

		private Version ReadVersion()
		{
			var major = _reader.ReadInt16();
			var minor = _reader.ReadInt16();

			return new Version(major, minor);
		}

		private string ReadString(int offset, int length)
		{
			var buffer = new byte[length];

			_stream.Seek(offset, SeekOrigin.Begin);
			if (_stream.Read(buffer, 0, length) != length)
				throw new InvalidOperationException("Couldn't read string at " + offset);

			return Encoding.GetString(buffer, 0, length);
		}

		public Translation ReadTranslation()
		{
			if (++_index >= StringCount)
				return null;

			_stream.Seek(OriginalTableOffset + 8 * _index, SeekOrigin.Begin);
			var originalLength = _reader.ReadInt32();
			var originalOffset = _reader.ReadInt32();

			_stream.Seek(TranslationTableOffset + 8 * _index, SeekOrigin.Begin);
			var translationLength = _reader.ReadInt32();
			var translationOffset = _reader.ReadInt32();

			var id = ReadString(originalOffset, originalLength);
			var translations = ReadString(translationOffset, translationLength)
				.Split('\0');

			string context;
			string singular;
			string plural;

			var contextSplit = id.Split(CONTEXT_SEPARATORS, 2, StringSplitOptions.RemoveEmptyEntries);
			if (contextSplit.Length == 0)
			{
				context = null;
				singular = "";
			}
			else if (contextSplit.Length == 1)
			{
				context = null;
				singular = contextSplit[0];
			}
			else if (contextSplit.Length == 2)
			{
				context = contextSplit[0];
				singular = contextSplit[1];
			}
			else
				throw new InvalidOperationException("Failed to parse translation context '" + id + "'");

			var pluralSplit = singular.Split(STRING_SEPARATORS, 2, StringSplitOptions.RemoveEmptyEntries);
			if (pluralSplit.Length == 0)
			{
				singular = "";
				plural = null;
			}
			else if (pluralSplit.Length == 1)
			{
				singular = pluralSplit[0];
				plural = null;
			}
			else if (pluralSplit.Length == 2)
			{
				singular = pluralSplit[0];
				plural = pluralSplit[1];
			}
			else
				throw new InvalidOperationException("Failed to parse translation plural form '" + id + "'");

			var key = new GettextKey(context, singular, plural);

			return new Translation(key, translations);
		}

		#region IDisposable

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_stream != null)
				{
					if (!_leaveOpen)
						_stream.Dispose();
					_stream = null;
				}

				if (_reader != null)
				{
					// reader respects "leaveOpen"
					_reader.Dispose();
					_reader = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}