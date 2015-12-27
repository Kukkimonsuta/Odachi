using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Gettext.IO
{
	public enum Endianness
	{
		Little,
		Big
	}

	public class BinaryReader : IDisposable
	{
		public BinaryReader(Stream stream, bool leaveOpen = false, Endianness endianness = Endianness.Little)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			_stream = stream;
			_leaveOpen = leaveOpen;
			_endianness = endianness;

			_buffer = new byte[8];
		}

		private Stream _stream;
		private bool _leaveOpen;
		private Endianness _endianness;

		private byte[] _buffer;

		public void Read(byte[] bytes)
		{
			Read(bytes, 0, bytes.Length);
		}
		public void Read(byte[] bytes, int index, int count)
		{
			var read = _stream.Read(bytes, index, count);
			if (read < count)
				throw new EndOfStreamException();
		}

		public byte ReadUInt8()
		{
			var result = _stream.ReadByte();
			if (result == -1)
				throw new EndOfStreamException();

			return (byte)_stream.ReadByte();
		}
		public sbyte ReadInt8()
		{
			var result = _stream.ReadByte();
			if (result == -1)
				throw new EndOfStreamException();

			return (sbyte)_stream.ReadByte();
		}

		public short ReadInt16()
		{
			Read(_buffer, 0, 2);

			if (_endianness == Endianness.Big)
			{
				return (short)
				(
					(short)((_buffer[0] << 8) & 0xff00) |
					(short)((_buffer[1] << 0) & 0xff)
				);
			}
			else
			{
				return (short)
				(
					(short)((_buffer[0] << 0) & 0xff) |
					(short)((_buffer[1] << 8) & 0xff00)
				);
			}
		}

		public int ReadInt32()
		{
			Read(_buffer, 0, 4);

			if (_endianness == Endianness.Big)
			{
				return (int)
				(
					(int)((_buffer[0] << 24) & 0xff000000) |
					(int)((_buffer[1] << 16) & 0xff0000) |
					(int)((_buffer[2] << 8) & 0xff00) |
					(int)((_buffer[3] << 0) & 0xff)
				);
			}
			else
			{
				return (int)
				(
					(int)((_buffer[0] << 0) & 0xff) |
					(int)((_buffer[1] << 8) & 0xff00) |
					(int)((_buffer[2] << 16) & 0xff0000) |
					(int)((_buffer[3] << 24) & 0xff000000)
				);
			}
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
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
