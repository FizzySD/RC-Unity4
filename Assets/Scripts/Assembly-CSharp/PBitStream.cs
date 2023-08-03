using System;
using System.Collections.Generic;

public class PBitStream
{
	private int currentByte;

	private List<byte> streamBytes;

	private int totalBits;

	public int BitCount
	{
		get
		{
			return totalBits;
		}
		private set
		{
			totalBits = value;
		}
	}

	public int ByteCount
	{
		get
		{
			return BytesForBits(totalBits);
		}
	}

	public int Position { get; set; }

	public PBitStream()
	{
		streamBytes = new List<byte>(1);
	}

	public PBitStream(int bitCount)
	{
		streamBytes = new List<byte>(BytesForBits(bitCount));
	}

	public PBitStream(IEnumerable<byte> bytes, int bitCount)
	{
		streamBytes = new List<byte>(bytes);
		BitCount = bitCount;
	}

	public void Add(bool val)
	{
		int num = totalBits / 8;
		if (num > streamBytes.Count - 1 || totalBits == 0)
		{
			streamBytes.Add(0);
		}
		if (val)
		{
			int num2 = 7 - totalBits % 8;
			streamBytes[num] |= (byte)(1 << num2);
		}
		totalBits++;
	}

	public static int BytesForBits(int bitCount)
	{
		if (bitCount <= 0)
		{
			return 0;
		}
		return (bitCount - 1) / 8 + 1;
	}

	public bool Get(int bitIndex)
	{
		int index = bitIndex / 8;
		int num = 7 - bitIndex % 8;
		return (streamBytes[index] & (byte)(1 << num)) > 0;
	}

	public bool GetNext()
	{
		if (Position > totalBits)
		{
			throw new Exception("End of PBitStream reached. Can't read more.");
		}
		return Get(Position++);
	}

	public void Set(int bitIndex, bool value)
	{
		int index = bitIndex / 8;
		int num = 7 - bitIndex % 8;
		streamBytes[index] |= (byte)(1 << num);
	}

	public byte[] ToBytes()
	{
		return streamBytes.ToArray();
	}
}
