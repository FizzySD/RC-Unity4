using Settings;

namespace UI
{
	internal class SettingsSkinsForestPanel : SettingsCategoryPanel
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
			BaseCustomSkinSettings<ForestCustomSkinSet> forest = SettingsManager.CustomSkinSettings.Forest;
			ForestCustomSkinSet forestCustomSkinSet = (ForestCustomSkinSet)forest.GetSelectedSet();
			string localeCategory = settingsPopup.LocaleCategory;
			string subCategory = "Skins.Forest";
			settingsSkinsPanel.CreateCommonSettings(DoublePanelLeft, DoublePanelRight);
			ElementFactory.CreateToggleSetting(DoublePanelRight, new ElementStyle(24, 200f, ThemePanel), forestCustomSkinSet.RandomizedPairs, UIManager.GetLocale(localeCategory, "Skins.Common", "RandomizedPairs"), UIManager.GetLocale(localeCategory, "Skins.Common", "RandomizedPairsTooltip"));
			CreateHorizontalDivider(DoublePanelLeft);
			CreateHorizontalDivider(DoublePanelRight);
			ElementFactory.CreateInputSetting(DoublePanelRight, new ElementStyle(24, 140f, ThemePanel), forestCustomSkinSet.Ground, UIManager.GetLocale(localeCategory, "Skins.Common", "Ground"), "", 260f);
			settingsSkinsPanel.CreateSkinListStringSettings(forestCustomSkinSet.TreeTrunks, DoublePanelLeft, UIManager.GetLocale(localeCategory, subCategory, "TreeTrunks"));
			settingsSkinsPanel.CreateSkinListStringSettings(forestCustomSkinSet.TreeLeafs, DoublePanelRight, UIManager.GetLocale(localeCategory, subCategory, "TreeLeafs"));
		}
	}
}
