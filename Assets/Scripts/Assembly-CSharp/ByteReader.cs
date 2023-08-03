using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ByteReader
{
	private byte[] mBuffer;

	private int mOffset;

	public bool canRead
	{
		get
		{
			if (mBuffer != null)
			{
				return mOffset < mBuffer.Length;
			}
			return false;
		}
	}

	public ByteReader(byte[] bytes)
	{
		mBuffer = bytes;
	}

	public ByteReader(TextAsset asset)
	{
		mBuffer = asset.bytes;
	}

	public Dictionary<string, string> ReadDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		char[] separator = new char[1] { '=' };
		while (canRead)
		{
			string text = ReadLine();
			if (text == null)
			{
				return dictionary;
			}
			if (!text.StartsWith("//"))
			{
				string[] array = text.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 2)
				{
					string key = array[0].Trim();
					string value = array[1].Trim().Replace("\\n", "\n");
					dictionary[key] = value;
				}
			}
		}
		return dictionary;
	}

	public string ReadLine()
	{
		int num = mBuffer.Length;
		while (mOffset < num && mBuffer[mOffset] < 32)
		{
			mOffset++;
		}
		int num2 = mOffset;
		if (num2 >= num)
		{
			mOffset = num;
			return null;
		}
		byte b;
		do
		{
			if (num2 < num)
			{
				b = mBuffer[num2++];
				continue;
			}
			num2++;
			break;
		}
		while (b != 10 && b != 13);
		string result = ReadLine(mBuffer, mOffset, num2 - mOffset - 1);
		mOffset = num2;
		return result;
	}

	private static string ReadLine(byte[] buffer, int start, int count)
	{
		return Encoding.UTF8.GetString(buffer, start, count);
	}
}
