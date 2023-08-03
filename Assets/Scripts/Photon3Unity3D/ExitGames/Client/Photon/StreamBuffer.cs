using System;
using System.IO;

namespace ExitGames.Client.Photon
{
	public class StreamBuffer
	{
		private const int DefaultInitialSize = 0;

		private int pos;

		private int len;

		private byte[] buf;

		public Guid guid;

		public int BackingArrayLength
		{
			get
			{
				return buf.Length;
			}
		}

		public bool CanRead
		{
			get
			{
				return true;
			}
		}

		public bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public int Length
		{
			get
			{
				return len;
			}
		}

		public int Position
		{
			get
			{
				return pos;
			}
			set
			{
				pos = value;
				if (len < pos)
				{
					len = pos;
					CheckSize(len);
				}
			}
		}

		public StreamBuffer(int size = 0)
		{
			buf = new byte[size];
		}

		public StreamBuffer(byte[] buf)
		{
			this.buf = buf;
			len = buf.Length;
		}

		public byte[] ToArray()
		{
			byte[] array = new byte[len];
			Buffer.BlockCopy(buf, 0, array, 0, len);
			return array;
		}

		public byte[] ToArrayFromPos()
		{
			int num = len - pos;
			if (num <= 0)
			{
				return new byte[0];
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(buf, pos, array, 0, num);
			return array;
		}

		public void Compact()
		{
			long num = Length - Position;
			if (num > 0)
			{
				Buffer.BlockCopy(buf, Position, buf, 0, (int)num);
			}
			Position = 0;
			SetLength(num);
		}

		public byte[] GetBuffer()
		{
			return buf;
		}

		public byte[] GetBufferAndAdvance(int length, out int offset)
		{
			offset = Position;
			Position += length;
			return buf;
		}

		public void Flush()
		{
		}

		public long Seek(long offset, SeekOrigin origin)
		{
			int num = 0;
			switch (origin)
			{
			case SeekOrigin.Begin:
				num = (int)offset;
				break;
			case SeekOrigin.Current:
				num = pos + (int)offset;
				break;
			case SeekOrigin.End:
				num = len + (int)offset;
				break;
			default:
				throw new ArgumentException("Invalid seek origin");
			}
			if (num < 0)
			{
				throw new ArgumentException("Seek before begin");
			}
			if (num > len)
			{
				throw new ArgumentException("Seek after end");
			}
			pos = num;
			return pos;
		}

		public void SetLength(long value)
		{
			len = (int)value;
			CheckSize(len);
			if (pos > len)
			{
				pos = len;
			}
		}

		public void SetCapacityMinimum(int neededSize)
		{
			CheckSize(neededSize);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = len - pos;
			if (num <= 0)
			{
				return 0;
			}
			if (count > num)
			{
				count = num;
			}
			Buffer.BlockCopy(buf, pos, buffer, offset, count);
			pos += count;
			return count;
		}

		public void Write(byte[] buffer, int srcOffset, int count)
		{
			int num = pos + count;
			CheckSize(num);
			if (num > len)
			{
				len = num;
			}
			Buffer.BlockCopy(buffer, srcOffset, buf, pos, count);
			pos = num;
		}

		public byte ReadByte()
		{
			if (pos >= len)
			{
				throw new EndOfStreamException("SteamBuffer.ReadByte() failed. pos:" + pos + "len:" + len);
			}
			return buf[pos++];
		}

		public void WriteByte(byte value)
		{
			if (pos >= len)
			{
				len = pos + 1;
				CheckSize(len);
			}
			buf[pos++] = value;
		}

		public void WriteBytes(byte v0, byte v1)
		{
			int num = pos + 2;
			if (len < num)
			{
				len = num;
				CheckSize(len);
			}
			buf[pos++] = v0;
			buf[pos++] = v1;
		}

		public void WriteBytes(byte v0, byte v1, byte v2)
		{
			int num = pos + 3;
			if (len < num)
			{
				len = num;
				CheckSize(len);
			}
			buf[pos++] = v0;
			buf[pos++] = v1;
			buf[pos++] = v2;
		}

		public void WriteBytes(byte v0, byte v1, byte v2, byte v3)
		{
			int num = pos + 4;
			if (len < num)
			{
				len = num;
				CheckSize(len);
			}
			buf[pos++] = v0;
			buf[pos++] = v1;
			buf[pos++] = v2;
			buf[pos++] = v3;
		}

		public void WriteBytes(byte v0, byte v1, byte v2, byte v3, byte v4, byte v5, byte v6, byte v7)
		{
			int num = pos + 8;
			if (len < num)
			{
				len = num;
				CheckSize(len);
			}
			buf[pos++] = v0;
			buf[pos++] = v1;
			buf[pos++] = v2;
			buf[pos++] = v3;
			buf[pos++] = v4;
			buf[pos++] = v5;
			buf[pos++] = v6;
			buf[pos++] = v7;
		}

		private bool CheckSize(int size)
		{
			if (size <= buf.Length)
			{
				return false;
			}
			int num = buf.Length;
			if (num == 0)
			{
				num = 1;
			}
			while (size > num)
			{
				num *= 2;
			}
			byte[] dst = new byte[num];
			Buffer.BlockCopy(buf, 0, dst, 0, buf.Length);
			buf = dst;
			return true;
		}
	}
}
