using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace UI
{
	internal class SettingsSkinsPanel : SettingsCategoryPanel
	{
		protected Dictionary<string, ICustomSkinSettings> _settings = new Dictionary<string, ICustomSkinSettings>();

		protected override bool CategoryPanel
		{
			get
			{
				return true;
			}
		}

		protected override string DefaultCategoryPanel
		{
			get
			{
				return "Human";
			}
		}

		public void CreateCommonSettings(Transform panelLeft, Transform panelRight)
		{
			ElementStyle style = new ElementStyle(24, 140f, ThemePanel);
			ElementStyle style2 = new ElementStyle(24, 200f, ThemePanel);
			ICustomSkinSettings currentSettings = GetCurrentSettings();
			string value = _currentCategoryPanelName.Value;
			string[] options = new string[7] { "Human", "Titan", "Shifter", "Skybox", "Forest", "City", "Custom level" };
			ElementFactory.CreateDropdownSetting(panelLeft, style, _currentCategoryPanelName, UIManager.GetLocaleCommon("Category"), options, "", 260f, 40f, 300f, null, delegate
			{
				RebuildCategoryPanel();
			});
			string subCategory = "Skins.Common";
			string localeCategory = ((SettingsPopup)Parent).LocaleCategory;
			ElementFactory.CreateDropdownSetting(panelLeft, style, currentSettings.GetSelectedSetIndex(), UIManager.GetLocale(localeCategory, subCategory, "Set"), currentSettings.GetSetNames(), "", 260f, 40f, 300f, null, delegate
			{
				RebuildCategoryPanel();
			});
			GameObject gameObject = ElementFactory.CreateHorizontalGroup(panelLeft, 10f, TextAnchor.UpperRight);
			string[] array = new string[4] { "Create", "Delete", "Rename", "Copy" };
			foreach (string button in array)
			{
				GameObject gameObject2 = ElementFactory.CreateDefaultButton(gameObject.transform, style, UIManager.GetLocaleCommon(button), 0f, 0f, delegate
				{
					OnSkinsPanelButtonClick(button);
				});
			}
			ElementFactory.CreateToggleSetting(panelRight, style2, currentSettings.GetSkinsEnabled(), value + " " + UIManager.GetLocale(localeCategory, "Skins.Common", "SkinsEnabled"));
			ElementFactory.CreateToggleSetting(panelRight, style2, currentSettings.GetSkinsLocal(), value + " " + UIManager.GetLocale(localeCategory, "Skins.Common", "SkinsLocal"), UIManager.GetLocale(localeCategory, "Skins.Common", "SkinsLocalTooltip"));
		}

		private void OnSkinsPanelButtonClick(string name)
		{
			SetNamePopup setNamePopup = ((SettingsPopup)Parent).SetNamePopup;
			ICustomSkinSettings customSkinSettings = _settings[_currentCategoryPanelName.Value];
			switch (name)
			{
			case "Create":
				setNamePopup.Show("New set", delegate
				{
					OnSkinsSetOperationFinish(name);
				}, UIManager.GetLocaleCommon("Create"));
				break;
			case "Delete":
				if (customSkinSettings.CanDeleteSelectedSet())
				{
					UIManager.CurrentMenu.ConfirmPopup.Show(UIManager.GetLocaleCommon("DeleteWarning"), delegate
					{
						OnSkinsSetOperationFinish(name);
					}, UIManager.GetLocaleCommon("Delete"));
				}
				break;
			case "Rename":
			{
				string value = customSkinSettings.GetSelectedSet().Name.Value;
				setNamePopup.Show(value, delegate
				{
					OnSkinsSetOperationFinish(name);
				}, UIManager.GetLocaleCommon("Rename"));
				break;
			}
			case "Copy":
				setNamePopup.Show("New set", delegate
				{
					OnSkinsSetOperationFinish(name);
				}, UIManager.GetLocaleCommon("Copy"));
				break;
			}
		}

		private void OnSkinsSetOperationFinish(string name)
		{
			SetNamePopup setNamePopup = ((SettingsPopup)Parent).SetNamePopup;
			ICustomSkinSettings currentSettings = GetCurrentSettings();
			switch (name)
			{
			case "Create":
				currentSettings.CreateSet(setNamePopup.NameSetting.Value);
				currentSettings.GetSelectedSetIndex().Value = currentSettings.GetSets().GetCount() - 1;
				break;
			case "Delete":
				currentSettings.DeleteSelectedSet();
				currentSettings.GetSelectedSetIndex().Value = 0;
				break;
			case "Rename":
				currentSettings.GetSelectedSet().Name.Value = setNamePopup.NameSetting.Value;
				break;
			case "Copy":
				currentSettings.CopySelectedSet(setNamePopup.NameSetting.Value);
				currentSettings.GetSelectedSetIndex().Value = currentSettings.GetSets().GetCount() - 1;
				break;
			}
			RebuildCategoryPanel();
		}

		public void CreateSkinListStringSettings(ListSetting<StringSetting> list, Transform panel, string title)
		{
			ElementStyle style = new ElementStyle(24, 0f, ThemePanel);
			ElementFactory.CreateDefaultLabel(panel, style, title);
			foreach (StringSetting item in list.Value)
			{
				ElementFactory.CreateInputSetting(panel, style, item, string.Empty, "", 420f);
			}
		}

		public void CreateSkinStringSettings(Transform panelLeft, Transform panelRight, float titleWidth = 140f, float elementWidth = 260f, int leftCount = 0)
		{
			ElementStyle style = new ElementStyle(24, titleWidth, ThemePanel);
			BaseSettingsContainer selectedSet = GetCurrentSettings().GetSelectedSet();
			string localeCategory = ((SettingsPopup)Parent).LocaleCategory;
			string text = "Skins." + _currentCategoryPanelName.Value;
			int num = 1;
			foreach (DictionaryEntry setting in selectedSet.Settings)
			{
				BaseSetting baseSetting = (BaseSetting)setting.Value;
				string text2 = (string)setting.Key;
				Transform parent = ((num <= leftCount) ? panelLeft : panelRight);
				if ((baseSetting.GetType() == typeof(StringSetting) || baseSetting.GetType() == typeof(FloatSetting)) && text2 != "Name")
				{
					string subCategory = text;
					if (text2 == "Ground")
					{
						subCategory = "Skins.Common";
					}
					ElementFactory.CreateInputSetting(parent, style, baseSetting, UIManager.GetLocale(localeCategory, subCategory, text2), "", elementWidth);
					num++;
				}
			}
		}

		public ICustomSkinSettings GetCurrentSettings()
		{
			return _settings[_currentCategoryPanelName.Value];
		}

		protected override void RegisterCategoryPanels()
		{
			_categoryPanelTypes.Add("Human", typeof(SettingsSkinsHumanPanel));
			_categoryPanelTypes.Add("Titan", typeof(SettingsSkinsTitanPanel));
			_categoryPanelTypes.Add("Forest", typeof(SettingsSkinsForestPanel));
			_categoryPanelTypes.Add("City", typeof(SettingsSkinsCityPanel));
			_categoryPanelTypes.Add("Shifter", typeof(SettingsSkinsDefaultPanel));
			_categoryPanelTypes.Add("Skybox", typeof(SettingsSkinsDefaultPanel));
			_categoryPanelTypes.Add("Custom level", typeof(SettingsSkinsDefaultPanel));
			_settings.Add("Human", SettingsManager.CustomSkinSettings.Human);
			_settings.Add("Titan", SettingsManager.CustomSkinSettings.Titan);
			_settings.Add("Forest", SettingsManager.CustomSkinSettings.Forest);
			_settings.Add("City", SettingsManager.CustomSkinSettings.City);
			_settings.Add("Shifter", SettingsManager.CustomSkinSettings.Shifter);
			_settings.Add("Skybox", SettingsManager.CustomSkinSettings.Skybox);
			_settings.Add("Custom level", SettingsManager.CustomSkinSettings.CustomLevel);
		}
	}
}
