using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BMFont
{
	[HideInInspector]
	[SerializeField]
	private int mBase;

	private Dictionary<int, BMGlyph> mDict = new Dictionary<int, BMGlyph>();

	[HideInInspector]
	[SerializeField]
	private int mHeight;

	[HideInInspector]
	[SerializeField]
	private List<BMGlyph> mSaved = new List<BMGlyph>();

	[HideInInspector]
	[SerializeField]
	private int mSize;

	[HideInInspector]
	[SerializeField]
	private string mSpriteName;

	[HideInInspector]
	[SerializeField]
	private int mWidth;

	public int baseOffset
	{
		get
		{
			return mBase;
		}
		set
		{
			mBase = value;
		}
	}

	public int charSize
	{
		get
		{
			return mSize;
		}
		set
		{
			mSize = value;
		}
	}

	public int glyphCount
	{
		get
		{
			if (isValid)
			{
				return mSaved.Count;
			}
			return 0;
		}
	}

	public bool isValid
	{
		get
		{
			return mSaved.Count > 0;
		}
	}

	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			mSpriteName = value;
		}
	}

	public int texHeight
	{
		get
		{
			return mHeight;
		}
		set
		{
			mHeight = value;
		}
	}

	public int texWidth
	{
		get
		{
			return mWidth;
		}
		set
		{
			mWidth = value;
		}
	}

	public void Clear()
	{
		mDict.Clear();
		mSaved.Clear();
	}

	public BMGlyph GetGlyph(int index)
	{
		return GetGlyph(index, false);
	}

	public BMGlyph GetGlyph(int index, bool createIfMissing)
	{
		BMGlyph value = null;
		if (mDict.Count == 0)
		{
			int i = 0;
			for (int count = mSaved.Count; i < count; i++)
			{
				BMGlyph bMGlyph = mSaved[i];
				mDict.Add(bMGlyph.index, bMGlyph);
			}
		}
		if (!mDict.TryGetValue(index, out value) && createIfMissing)
		{
			value = new BMGlyph
			{
				index = index
			};
			mSaved.Add(value);
			mDict.Add(index, value);
		}
		return value;
	}

	public void Trim(int xMin, int yMin, int xMax, int yMax)
	{
		if (!isValid)
		{
			return;
		}
		int i = 0;
		for (int count = mSaved.Count; i < count; i++)
		{
			BMGlyph bMGlyph = mSaved[i];
			if (bMGlyph != null)
			{
				bMGlyph.Trim(xMin, yMin, xMax, yMax);
			}
		}
	}
}
