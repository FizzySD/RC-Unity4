using System.Collections.Generic;
using UnityEngine;

public abstract class UIItemSlot : MonoBehaviour
{
	public UIWidget background;

	public AudioClip errorSound;

	public AudioClip grabSound;

	public UISprite icon;

	public UILabel label;

	private static InvGameItem mDraggedItem;

	private InvGameItem mItem;

	private string mText = string.Empty;

	public AudioClip placeSound;

	protected abstract InvGameItem observedItem { get; }

	private void OnClick()
	{
		if (mDraggedItem != null)
		{
			OnDrop(null);
		}
		else if (mItem != null)
		{
			mDraggedItem = Replace(null);
			if (mDraggedItem != null)
			{
				NGUITools.PlaySound(grabSound);
			}
			UpdateCursor();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (mDraggedItem == null && mItem != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			mDraggedItem = Replace(null);
			NGUITools.PlaySound(grabSound);
			UpdateCursor();
		}
	}

	private void OnDrop(GameObject go)
	{
		InvGameItem invGameItem = Replace(mDraggedItem);
		if (mDraggedItem == invGameItem)
		{
			NGUITools.PlaySound(errorSound);
		}
		else if (invGameItem != null)
		{
			NGUITools.PlaySound(grabSound);
		}
		else
		{
			NGUITools.PlaySound(placeSound);
		}
		mDraggedItem = invGameItem;
		UpdateCursor();
	}

	private void OnTooltip(bool show)
	{
		InvGameItem invGameItem = ((!show) ? null : mItem);
		if (invGameItem != null)
		{
			InvBaseItem baseItem = invGameItem.baseItem;
			if (baseItem != null)
			{
				string[] array = new string[5]
				{
					"[",
					NGUITools.EncodeColor(invGameItem.color),
					"]",
					invGameItem.name,
					"[-]\n"
				};
				string text = string.Concat(array);
				object[] array2 = new object[5] { text, "[AFAFAF]Level ", invGameItem.itemLevel, " ", baseItem.slot };
				string text2 = string.Concat(array2);
				List<InvStat> list = invGameItem.CalculateStats();
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					InvStat invStat = list[i];
					if (invStat.amount != 0)
					{
						text2 = ((invStat.amount >= 0) ? (text2 + "\n[00FF00]+" + invStat.amount) : (text2 + "\n[FF0000]" + invStat.amount));
						if (invStat.modifier == InvStat.Modifier.Percent)
						{
							text2 += "%";
						}
						text2 = text2 + " " + invStat.id.ToString() + "[-]";
					}
				}
				if (!string.IsNullOrEmpty(baseItem.description))
				{
					text2 = text2 + "\n[FF9900]" + baseItem.description;
				}
				UITooltip.ShowText(text2);
				return;
			}
		}
		UITooltip.ShowText(null);
	}

	protected abstract InvGameItem Replace(InvGameItem item);

	private void Update()
	{
		InvGameItem invGameItem = observedItem;
		if (mItem == invGameItem)
		{
			return;
		}
		mItem = invGameItem;
		InvBaseItem invBaseItem = ((invGameItem == null) ? null : invGameItem.baseItem);
		if (label != null)
		{
			string text = ((invGameItem == null) ? null : invGameItem.name);
			if (string.IsNullOrEmpty(mText))
			{
				mText = label.text;
			}
			label.text = ((text == null) ? mText : text);
		}
		if (icon != null)
		{
			if (invBaseItem == null || invBaseItem.iconAtlas == null)
			{
				icon.enabled = false;
			}
			else
			{
				icon.atlas = invBaseItem.iconAtlas;
				icon.spriteName = invBaseItem.iconName;
				icon.enabled = true;
				icon.MakePixelPerfect();
			}
		}
		if (background != null)
		{
			background.color = ((invGameItem == null) ? Color.white : invGameItem.color);
		}
	}

	private void UpdateCursor()
	{
		if (mDraggedItem != null && mDraggedItem.baseItem != null)
		{
			UICursor.Set(mDraggedItem.baseItem.iconAtlas, mDraggedItem.baseItem.iconName);
		}
		else
		{
			UICursor.Clear();
		}
	}
}
