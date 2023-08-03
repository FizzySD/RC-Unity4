using Settings;

namespace UI
{
	internal class SettingsUIPanel : SettingsCategoryPanel
	{
		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			SettingsPopup settingsPopup = (SettingsPopup)parent;
			string localeCategory = settingsPopup.LocaleCategory;
			string subCategory = "UI";
			UISettings uISettings = SettingsManager.UISettings;
			ElementStyle style = new ElementStyle(24, 200f, ThemePanel);
			ElementFactory.CreateDropdownSetting(DoublePanelLeft, style, SettingsManager.UISettings.UITheme, UIManager.GetLocale(localeCategory, subCategory, "Theme"), UIManager.GetUIThemes(), UIManager.GetLocaleCommon("RequireRestart"), 160f);
			ElementFactory.CreateSliderSetting(DoublePanelLeft, style, SettingsManager.UISettings.UIMasterScale, UIManager.GetLocale(localeCategory, subCategory, "UIScale"), "", 135f);
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, SettingsManager.UISettings.GameFeed, UIManager.GetLocale(localeCategory, subCategory, "GameFeed"), UIManager.GetLocale(localeCategory, subCategory, "GameFeedTooltip"));
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, SettingsManager.UISettings.ShowEmotes, UIManager.GetLocale(localeCategory, subCategory, "ShowEmotes"));
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, SettingsManager.UISettings.ShowInterpolation, UIManager.GetLocale(localeCategory, subCategory, "ShowInterpolation"), UIManager.GetLocale(localeCategory, subCategory, "ShowInterpolationTooltip"));
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, SettingsManager.UISettings.HideNames, UIManager.GetLocale(localeCategory, subCategory, "HideNames"));
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, SettingsManager.UISettings.DisableNameColors, UIManager.GetLocale(localeCategory, subCategory, "DisableNameColors"));
			ElementFactory.CreateDropdownSetting(DoublePanelRight, style, SettingsManager.UISettings.CrosshairStyle, UIManager.GetLocale(localeCategory, subCategory, "CrosshairStyle"), UIManager.GetLocaleArray(localeCategory, subCategory, "CrosshairStyleOptions"), "", 200f);
			ElementFactory.CreateSliderSetting(DoublePanelRight, new ElementStyle(24, 150f, ThemePanel), SettingsManager.UISettings.CrosshairScale, UIManager.GetLocale(localeCategory, subCategory, "CrosshairScale"), "", 185f);
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, SettingsManager.UISettings.ShowCrosshairDistance, UIManager.GetLocale(localeCategory, subCategory, "ShowCrosshairDistance"));
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, SettingsManager.UISettings.ShowCrosshairArrows, UIManager.GetLocale(localeCategory, subCategory, "ShowCrosshairArrows"));
			ElementFactory.CreateToggleGroupSetting(DoublePanelRight, style, SettingsManager.UISettings.Speedometer, UIManager.GetLocale(localeCategory, subCategory, "Speedometer"), UIManager.GetLocaleArray(localeCategory, subCategory, "SpeedometerOptions"));
		}
	}
}
