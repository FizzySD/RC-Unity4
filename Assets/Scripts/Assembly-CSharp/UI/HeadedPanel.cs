using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class HeadedPanel : BasePanel
	{
		protected Transform BottomBar;

		protected Transform TopBar;

		protected Dictionary<string, Button> _topButtons = new Dictionary<string, Button>();

		protected virtual string Title
		{
			get
			{
				return "Default";
			}
		}

		protected virtual float TopBarHeight
		{
			get
			{
				return 65f;
			}
		}

		protected virtual float BottomBarHeight
		{
			get
			{
				return 65f;
			}
		}

		protected override float BorderVerticalPadding
		{
			get
			{
				return 5f;
			}
		}

		protected override float BorderHorizontalPadding
		{
			get
			{
				return 5f;
			}
		}

		protected override int VerticalPadding
		{
			get
			{
				return 25;
			}
		}

		protected override int HorizontalPadding
		{
			get
			{
				return 35;
			}
		}

		protected virtual int TitleFontSize
		{
			get
			{
				return 30;
			}
		}

		protected virtual int ButtonFontSize
		{
			get
			{
				return 28;
			}
		}

		protected virtual bool CategoryButtons
		{
			get
			{
				return false;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			TopBar = base.transform.Find("Background/TopBar");
			BottomBar = base.transform.Find("Background/BottomBar");
			Transform transform = base.transform.Find("Background/TopBarLine");
			Transform transform2 = base.transform.Find("Background/BottomBarLine");
			TopBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TopBarHeight);
			BottomBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BottomBarHeight);
			transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f - TopBarHeight);
			transform2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, BottomBarHeight);
			if (TopBar.Find("Label") != null)
			{
				if (CategoryButtons)
				{
					TopBar.Find("Label").gameObject.SetActive(false);
				}
				else
				{
					TopBar.Find("Label").GetComponent<Text>().fontSize = TitleFontSize;
					TopBar.Find("Label").GetComponent<Text>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "TitleColor");
					SetTitle(Title);
				}
			}
			TopBar.GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "TopBarColor");
			BottomBar.GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "BottomBarColor");
			transform.GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "BorderColor");
			transform2.GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "BorderColor");
			base.transform.Find("Border").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "BorderColor");
			base.transform.Find("Background").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "BackgroundColor");
			base.Setup(parent);
			if (CategoryButtons)
			{
				SetupTopButtons();
				SetTopButton(_currentCategoryPanelName.Value);
			}
		}

		public override void SetCategoryPanel(string name)
		{
			base.SetCategoryPanel(name);
			SetTopButton(name);
		}

		protected virtual void SetTopButton(string name)
		{
			if (_topButtons.Count <= 0)
			{
				return;
			}
			foreach (Button value in _topButtons.Values)
			{
				value.interactable = true;
			}
			_topButtons[name].interactable = false;
		}

		protected void SetTitle(string title)
		{
			TopBar.Find("Label").GetComponent<Text>().text = title;
		}

		protected virtual void SetupTopButtons()
		{
			Canvas.ForceUpdateCanvases();
			float num = 0f;
			foreach (Button value in _topButtons.Values)
			{
				num += value.GetComponent<RectTransform>().rect.width;
			}
			TopBar.GetComponent<HorizontalLayoutGroup>().spacing = (Width - num) / (float)(_topButtons.Count + 1);
		}

		protected override float GetPanelHeight()
		{
			float y = TopBar.GetComponent<RectTransform>().sizeDelta.y;
			float y2 = BottomBar.GetComponent<RectTransform>().sizeDelta.y;
			return Height - y - y2 - BorderVerticalPadding * 2f;
		}
	}
}
