using System.Collections.Generic;
using ApplicationManagers;
using GameProgress;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class QuestCategoryPanel : BasePanel
	{
		protected float QuestItemWidth = 940f;

		protected float QuestItemHeight = 100f;

		protected override string ThemePanel
		{
			get
			{
				return "QuestPopup";
			}
		}

		protected override float Width
		{
			get
			{
				return 980f;
			}
		}

		protected override float Height
		{
			get
			{
				return 600f;
			}
		}

		protected override float VerticalSpacing
		{
			get
			{
				return 20f;
			}
		}

		protected override int HorizontalPadding
		{
			get
			{
				return 20;
			}
		}

		protected override int VerticalPadding
		{
			get
			{
				return 20;
			}
		}

		protected override TextAnchor PanelAlignment
		{
			get
			{
				return TextAnchor.UpperCenter;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
		}

		protected void CreateQuestItems(List<QuestItem> items)
		{
			foreach (QuestItem item in items)
			{
				Transform transform = ElementFactory.InstantiateAndBind(SinglePanel, "QuestItemPanel").transform;
				transform.GetComponent<LayoutElement>().preferredWidth = QuestItemWidth;
				transform.GetComponent<LayoutElement>().preferredHeight = QuestItemHeight;
				transform.Find("Panel/Icon").GetComponent<RawImage>().texture = (Texture2D)AssetBundleManager.LoadAsset(item.Icon.Value + "Icon", true);
				SetTitle(item, transform);
				SetRewardLabel(item, transform);
				SetProgress(item, transform);
				transform.Find("Background").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "BackgroundColor");
				transform.Find("Panel/CheckIcon").gameObject.SetActive(item.Finished());
				transform.Find("Border").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "BorderColor");
				transform.Find("Panel/Icon").GetComponent<RawImage>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "IconColor");
				transform.Find("Panel/CheckIcon").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "CheckColor");
				transform.Find("Panel/Title").GetComponent<Text>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "TextColor");
				transform.Find("Panel/ProgressLabel").GetComponent<Text>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "TextColor");
				transform.Find("Panel/RewardLabel").GetComponent<Text>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "TextColor");
				transform.Find("Panel/CheckIcon").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "IconColor");
				transform.Find("Panel/ProgressBar/Background").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "ProgressBarBackgroundColor");
				transform.Find("Panel/ProgressBar/Fill Area/Fill").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "QuestItem", "ProgressBarFillColor");
			}
		}

		protected void SetRewardLabel(QuestItem item, Transform panel)
		{
			if (item is AchievmentItem)
			{
				panel.Find("Panel/RewardLabel").gameObject.SetActive(false);
				panel.Find("Panel/AchievmentIcon").gameObject.SetActive(true);
				panel.Find("Panel/AchievmentIcon").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "Trophy", ((AchievmentItem)item).Tier.Value + "Color");
				return;
			}
			panel.Find("Panel/RewardLabel").gameObject.SetActive(true);
			panel.Find("Panel/AchievmentIcon").gameObject.SetActive(false);
			if (item.RewardType.Value == "Exp")
			{
				panel.Find("Panel/RewardLabel").GetComponent<Text>().text = "+" + item.RewardValue.Value + " exp";
			}
		}

		protected void SetTitle(QuestItem item, Transform panel)
		{
			string value = item.Category.Value;
			string locale = UIManager.GetLocale("QuestItems", value);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (StringSetting item2 in item.Conditions.Value)
			{
				string[] array = item2.Value.Split(':');
				dictionary.Add(array[0], array[1]);
			}
			string text = "";
			for (int i = 0; i < locale.Length; i++)
			{
				if (locale[i] == '{')
				{
					text += HandleConditionVariable(locale, i, dictionary);
					i = locale.IndexOf('}', i);
				}
				else if (locale[i] == '[')
				{
					int num = locale.IndexOf(']', i);
					int num2 = locale.IndexOf('{', i);
					int num3 = locale.IndexOf('}', i);
					string text2 = HandleConditionVariable(locale, num2, dictionary);
					if (text2 != string.Empty)
					{
						text = text + locale.Substring(i + 1, num2 - i - 1) + text2 + locale.Substring(num3 + 1, num - num3 - 1);
					}
					i = num;
				}
				else
				{
					text += locale[i];
				}
			}
			panel.Find("Panel/Title").GetComponent<Text>().text = text;
		}

		private string HandleConditionVariable(string locale, int index, Dictionary<string, string> conditionToValue)
		{
			int num = locale.IndexOf('}', index);
			string text = locale.Substring(index + 1, num - index - 1);
			if (conditionToValue.ContainsKey(text))
			{
				string locale2 = UIManager.GetLocale("QuestItems", text + "." + conditionToValue[text], "", "", "Error");
				if (locale2 == "Error")
				{
					return conditionToValue[text];
				}
				return locale2;
			}
			return string.Empty;
		}

		protected void SetProgress(QuestItem item, Transform panel)
		{
			panel.Find("Panel/ProgressBar").GetComponent<Slider>().value = (float)item.Progress.Value / (float)item.Amount.Value;
			panel.Find("Panel/ProgressLabel").GetComponent<Text>().text = item.Progress.Value + " / " + item.Amount.Value;
		}
	}
}
