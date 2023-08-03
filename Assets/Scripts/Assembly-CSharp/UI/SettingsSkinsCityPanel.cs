using Settings;

namespace UI
{
	internal class SettingsSkinsCityPanel : SettingsCategoryPanel
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
			BaseCustomSkinSettings<CityCustomSkinSet> city = SettingsManager.CustomSkinSettings.City;
			CityCustomSkinSet cityCustomSkinSet = (CityCustomSkinSet)city.GetSelectedSet();
			string localeCategory = settingsPopup.LocaleCategory;
			string subCategory = "Skins.City";
			ElementStyle style = new ElementStyle(24, 140f, ThemePanel);
			settingsSkinsPanel.CreateCommonSettings(DoublePanelLeft, DoublePanelRight);
			CreateHorizontalDivider(DoublePanelLeft);
			CreateHorizontalDivider(DoublePanelRight);
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, cityCustomSkinSet.Ground, UIManager.GetLocale(localeCategory, "Skins.Common", "Ground"), "", 260f);
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, cityCustomSkinSet.Wall, UIManager.GetLocale(localeCategory, subCategory, "Wall"), "", 260f);
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, cityCustomSkinSet.Gate, UIManager.GetLocale(localeCategory, subCategory, "Gate"), "", 260f);
			settingsSkinsPanel.CreateSkinListStringSettings(cityCustomSkinSet.Houses, DoublePanelRight, UIManager.GetLocale(localeCategory, subCategory, "Houses"));
		}
	}
}
