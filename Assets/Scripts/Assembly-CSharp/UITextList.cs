using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
	protected class Paragraph
	{
		public string[] lines;

		public string text;
	}

	public enum Style
	{
		Text = 0,
		Chat = 1
	}

	public int maxEntries = 50;

	public float maxHeight;

	public float maxWidth;

	protected List<Paragraph> mParagraphs = new List<Paragraph>();

	protected float mScroll;

	protected bool mSelected;

	protected char[] mSeparator = new char[1] { '\n' };

	protected int mTotalLines;

	public Style style;

	public bool supportScrollWheel = true;

	public UILabel textLabel;

	public void Add(string text)
	{
		Add(text, true);
	}

	protected void Add(string text, bool updateVisible)
	{
		Paragraph paragraph = null;
		if (mParagraphs.Count < maxEntries)
		{
			paragraph = new Paragraph();
		}
		else
		{
			paragraph = mParagraphs[0];
			mParagraphs.RemoveAt(0);
		}
		paragraph.text = text;
		mParagraphs.Add(paragraph);
		if (textLabel != null && textLabel.font != null)
		{
			paragraph.lines = textLabel.font.WrapText(paragraph.text, maxWidth / textLabel.transform.localScale.y, textLabel.maxLineCount, textLabel.supportEncoding, textLabel.symbolStyle).Split(mSeparator);
			mTotalLines = 0;
			int i = 0;
			for (int count = mParagraphs.Count; i < count; i++)
			{
				mTotalLines += mParagraphs[i].lines.Length;
			}
		}
		if (updateVisible)
		{
			UpdateVisibleText();
		}
	}

	private void Awake()
	{
		if (textLabel == null)
		{
			textLabel = GetComponentInChildren<UILabel>();
		}
		if (textLabel != null)
		{
			textLabel.lineWidth = 0;
		}
		Collider collider = base.collider;
		if (collider != null)
		{
			if (maxHeight <= 0f)
			{
				maxHeight = collider.bounds.size.y / base.transform.lossyScale.y;
			}
			if (maxWidth <= 0f)
			{
				maxWidth = collider.bounds.size.x / base.transform.lossyScale.x;
			}
		}
	}

	public void Clear()
	{
		mParagraphs.Clear();
		UpdateVisibleText();
	}

	private void OnScroll(float val)
	{
		if (mSelected && supportScrollWheel)
		{
			val *= ((style != Style.Chat) ? (-10f) : 10f);
			mScroll = Mathf.Max(0f, mScroll + val);
			UpdateVisibleText();
		}
	}

	private void OnSelect(bool selected)
	{
		mSelected = selected;
	}

	protected void UpdateVisibleText()
	{
		if (!(textLabel != null) || !(textLabel.font != null))
		{
			return;
		}
		int num = 0;
		int num2 = ((maxHeight <= 0f) ? 100000 : Mathf.FloorToInt(maxHeight / textLabel.cachedTransform.localScale.y));
		int num3 = Mathf.RoundToInt(mScroll);
		if (num2 + num3 > mTotalLines)
		{
			num3 = Mathf.Max(0, mTotalLines - num2);
			mScroll = num3;
		}
		if (style == Style.Chat)
		{
			num3 = Mathf.Max(0, mTotalLines - num2 - num3);
		}
		StringBuilder stringBuilder = new StringBuilder();
		int i = 0;
		for (int count = mParagraphs.Count; i < count; i++)
		{
			Paragraph paragraph = mParagraphs[i];
			int j = 0;
			for (int num4 = paragraph.lines.Length; j < num4; j++)
			{
				string value = paragraph.lines[j];
				if (num3 > 0)
				{
					num3--;
					continue;
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("\n");
				}
				stringBuilder.Append(value);
				num++;
				if (num >= num2)
				{
					break;
				}
			}
			if (num >= num2)
			{
				break;
			}
		}
		textLabel.text = stringBuilder.ToString();
	}
}
