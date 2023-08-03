using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite Animation")]
[RequireComponent(typeof(UISprite))]
[ExecuteInEditMode]
public class UISpriteAnimation : MonoBehaviour
{
	private bool mActive = true;

	private float mDelta;

	[HideInInspector]
	[SerializeField]
	private int mFPS = 30;

	private int mIndex;

	[SerializeField]
	[HideInInspector]
	private bool mLoop = true;

	[HideInInspector]
	[SerializeField]
	private string mPrefix = string.Empty;

	private UISprite mSprite;

	private List<string> mSpriteNames = new List<string>();

	public int frames
	{
		get
		{
			return mSpriteNames.Count;
		}
	}

	public int framesPerSecond
	{
		get
		{
			return mFPS;
		}
		set
		{
			mFPS = value;
		}
	}

	public bool isPlaying
	{
		get
		{
			return mActive;
		}
	}

	public bool loop
	{
		get
		{
			return mLoop;
		}
		set
		{
			mLoop = value;
		}
	}

	public string namePrefix
	{
		get
		{
			return mPrefix;
		}
		set
		{
			if (mPrefix != value)
			{
				mPrefix = value;
				RebuildSpriteList();
			}
		}
	}

	private void RebuildSpriteList()
	{
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
		}
		mSpriteNames.Clear();
		if (!(mSprite != null) || !(mSprite.atlas != null))
		{
			return;
		}
		List<UIAtlas.Sprite> spriteList = mSprite.atlas.spriteList;
		int i = 0;
		for (int count = spriteList.Count; i < count; i++)
		{
			UIAtlas.Sprite sprite = spriteList[i];
			if (string.IsNullOrEmpty(mPrefix) || sprite.name.StartsWith(mPrefix))
			{
				mSpriteNames.Add(sprite.name);
			}
		}
		mSpriteNames.Sort();
	}

	public void Reset()
	{
		mActive = true;
		mIndex = 0;
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			mSprite.MakePixelPerfect();
		}
	}

	private void Start()
	{
		RebuildSpriteList();
	}

	private void Update()
	{
		if (!mActive || mSpriteNames.Count <= 1 || !Application.isPlaying || !((float)mFPS > 0f))
		{
			return;
		}
		mDelta += Time.deltaTime;
		float num = 1f / (float)mFPS;
		if (num < mDelta)
		{
			mDelta = ((num <= 0f) ? 0f : (mDelta - num));
			if (++mIndex >= mSpriteNames.Count)
			{
				mIndex = 0;
				mActive = loop;
			}
			if (mActive)
			{
				mSprite.spriteName = mSpriteNames[mIndex];
				mSprite.MakePixelPerfect();
			}
		}
	}
}
