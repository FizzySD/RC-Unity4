/*
using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Examples/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
	public int charsPerSecond = 40;

	private UILabel mLabel;

	private float mNextChar;

	private int mOffset;

	private string mText;

	private void Update()
	{
		if (mLabel == null)
		{
			mLabel = GetComponent<UILabel>();
			mLabel.supportEncoding = false;
			mLabel.symbolStyle = UIFont.SymbolStyle.None;
			mText = mLabel.font.WrapText(mLabel.text, (float)mLabel.lineWidth / mLabel.cachedTransform.localScale.x, mLabel.maxLineCount, false, UIFont.SymbolStyle.None);
		}
		if (mOffset < mText.Length)
		{
			if (mNextChar <= Time.time)
			{
				charsPerSecond = Mathf.Max(1, charsPerSecond);
				float num = 1f / (float)charsPerSecond;
				switch (mText[mOffset])
				{
				case '\n':
				case '!':
				case '.':
				case '?':
					num *= 4f;
					break;
				}
				mNextChar = Time.time + num;
				mLabel.text = mText.Substring(0, ++mOffset);
			}
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
*/