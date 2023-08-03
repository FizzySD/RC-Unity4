using Settings;

namespace UI
{
	internal class SettingsGeneralPanel : SettingsCategoryPanel
	{
		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			SettingsPopup settingsPopup = (SettingsPopup)parent;
			string localeCategory = settingsPopup.LocaleCategory;
			string subCategory = "General";
			GeneralSettings generalSettings = SettingsManager.GeneralSettings;
			ElementStyle style = new ElementStyle(24, 200f, ThemePanel);
			ElementFactory.CreateDropdownSetting(DoublePanelLeft, style, generalSettings.Language, "Language", UIManager.GetLanguages(), UIManager.GetLocaleCommon("RequireRestart"), 160f, 40f, 300f, null, delegate
			{
				settingsPopup.RebuildCategoryPanel();
			});
			ElementFactory.CreateSliderSetting(DoublePanelLeft, style, generalSettings.Volume, UIManager.GetLocale(localeCategory, subCategory, "Volume"), "", 135f);
			ElementFactory.CreateSliderSetting(DoublePanelLeft, style, generalSettings.MouseSpeed, UIManager.GetLocale(localeCategory, subCategory, "MouseSpeed"), "", 135f);
			ElementFactory.CreateSliderSetting(DoublePanelLeft, style, generalSettings.CameraDistance, UIManager.GetLocale(localeCategory, subCategory, "CameraDistance"), "", 135f);
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, generalSettings.InvertMouse, UIManager.GetLocale(localeCategory, subCategory, "InvertMouse"));
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, generalSettings.CameraTilt, UIManager.GetLocale(localeCategory, subCategory, "CameraTilt"));
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, generalSettings.MinimapEnabled, UIManager.GetLocale(localeCategory, subCategory, "MinimapEnabled"));
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, generalSettings.SnapshotsEnabled, UIManager.GetLocale(localeCategory, subCategory, "SnapshotsEnabled"));
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, generalSettings.SnapshotsShowInGame, UIManager.GetLocale(localeCategory, subCategory, "SnapshotsShowInGame"));
			ElementFactory.CreateInputSetting(DoublePanelRight, style, generalSettings.SnapshotsMinimumDamage, UIManager.GetLocale(localeCategory, subCategory, "SnapshotsMinimumDamage"), "", 100f);
		}
	}
}
