using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace UI
{
	internal class SettingsSkinsTitanPanel : SettingsCategoryPanel
	{
		protected override float VerticalSpacing
		{
			get
			{
				return 20f;
			}
		}

		protected override bool ScrollBar
		{
			get
			{
				return true;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			SettingsSkinsPanel settingsSkinsPanel = (SettingsSkinsPanel)parent;
			SettingsPopup settingsPopup = (SettingsPopup)settingsSkinsPanel.Parent;
			BaseCustomSkinSettings<TitanCustomSkinSet> titan = SettingsManager.CustomSkinSettings.Titan;
			TitanCustomSkinSet titanCustomSkinSet = (TitanCustomSkinSet)titan.GetSelectedSet();
			string localeCategory = settingsPopup.LocaleCategory;
			string subCategory = "Skins.Titan";
			settingsSkinsPanel.CreateCommonSettings(DoublePanelLeft, DoublePanelRight);
			ElementStyle elementStyle = new ElementStyle(24, 200f, ThemePanel);
			ElementFactory.CreateToggleSetting(DoublePanelRight, elementStyle, titanCustomSkinSet.RandomizedPairs, UIManager.GetLocale(localeCategory, "Skins.Common", "RandomizedPairs"), UIManager.GetLocale(localeCategory, "Skins.Common", "RandomizedPairsTooltip"));
			CreateHorizontalDivider(DoublePanelLeft);
			CreateHorizontalDivider(DoublePanelRight);
			ElementFactory.CreateDefaultLabel(DoublePanelLeft, elementStyle, UIManager.GetLocale(localeCategory, subCategory, "Hairs"));
			List<string> list = new List<string> { "Random" };
			for (int i = 0; i < 10; i++)
			{
				list.Add("Hair " + i);
			}
			string[] options = list.ToArray();
			elementStyle.TitleWidth = 0f;
			for (int j = 0; j < titanCustomSkinSet.Hairs.GetCount(); j++)
			{
				GameObject gameObject = ElementFactory.CreateHorizontalGroup(DoublePanelLeft, 20f);
				ElementFactory.CreateInputSetting(gameObject.transform, elementStyle, titanCustomSkinSet.Hairs.GetItemAt(j), string.Empty, "", 260f);
				ElementFactory.CreateDropdownSetting(gameObject.transform, elementStyle, titanCustomSkinSet.HairModels.GetItemAt(j), string.Empty, options);
			}
			settingsSkinsPanel.CreateSkinListStringSettings(titanCustomSkinSet.Bodies, DoublePanelRight, UIManager.GetLocale(localeCategory, subCategory, "Bodies"));
			CreateHorizontalDivider(DoublePanelRight);
			settingsSkinsPanel.CreateSkinListStringSettings(titanCustomSkinSet.Eyes, DoublePanelRight, UIManager.GetLocale(localeCategory, subCategory, "Eyes"));
		}
	}
}
