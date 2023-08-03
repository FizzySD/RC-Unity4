using System.Collections;
using Settings;
using UnityEngine;

namespace UI
{
	internal class SettingsKeybindsDefaultPanel : SettingsCategoryPanel
	{
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
			SettingsKeybindsPanel settingsKeybindsPanel = (SettingsKeybindsPanel)parent;
			SettingsPopup settingsPopup = (SettingsPopup)settingsKeybindsPanel.Parent;
			settingsKeybindsPanel.CreateGategoryDropdown(DoublePanelLeft);
			string localeCategory = settingsPopup.LocaleCategory;
			ElementStyle style = new ElementStyle(24, 140f, ThemePanel);
			string text = settingsKeybindsPanel.GetCurrentCategoryName().Replace(" ", "");
			BaseSettingsContainer container = (BaseSettingsContainer)SettingsManager.InputSettings.Settings[text];
			CreateKeybindSettings(container, settingsPopup.KeybindPopup, localeCategory, "Keybinds." + text, style);
			if (text == "Human")
			{
				ElementFactory.CreateToggleSetting(DoublePanelRight, style, SettingsManager.InputSettings.Human.DashDoubleTap, UIManager.GetLocale(localeCategory, "Keybinds.Human", "DashDoubleTap"));
				ElementFactory.CreateSliderSetting(DoublePanelRight, style, SettingsManager.InputSettings.Human.ReelOutScrollSmoothing, UIManager.GetLocale(localeCategory, "Keybinds.Human", "ReelOutScrollSmoothing"), UIManager.GetLocale(localeCategory, "Keybinds.Human", "ReelOutScrollSmoothingTooltip"), 130f);
			}
		}

		private void CreateKeybindSettings(BaseSettingsContainer container, KeybindPopup popup, string cat, string sub, ElementStyle style)
		{
			int num = 0;
			foreach (DictionaryEntry setting in container.Settings)
			{
				BaseSetting baseSetting = (BaseSetting)setting.Value;
				string item = (string)setting.Key;
				if (baseSetting.GetType() == typeof(KeybindSetting))
				{
					Transform parent = ((num < container.Settings.Count / 2) ? DoublePanelLeft : DoublePanelRight);
					GameObject gameObject = ElementFactory.CreateKeybindSetting(parent, style, baseSetting, UIManager.GetLocale(cat, sub, item), popup);
					num++;
				}
			}
		}
	}
}
