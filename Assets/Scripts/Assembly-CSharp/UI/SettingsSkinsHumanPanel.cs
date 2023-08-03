using Settings;

namespace UI
{
	internal class SettingsSkinsHumanPanel : SettingsCategoryPanel
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
			HumanCustomSkinSettings human = SettingsManager.CustomSkinSettings.Human;
			settingsSkinsPanel.CreateCommonSettings(DoublePanelLeft, DoublePanelRight);
			ElementFactory.CreateToggleSetting(DoublePanelRight, new ElementStyle(24, 200f, ThemePanel), human.GasEnabled, UIManager.GetLocale(settingsPopup.LocaleCategory, "Skins.Human", "GasEnabled"));
			ElementFactory.CreateToggleSetting(DoublePanelRight, new ElementStyle(24, 200f, ThemePanel), human.HookEnabled, UIManager.GetLocale(settingsPopup.LocaleCategory, "Skins.Human", "HookEnabled"));
			CreateHorizontalDivider(DoublePanelLeft);
			CreateHorizontalDivider(DoublePanelRight);
			settingsSkinsPanel.CreateSkinStringSettings(DoublePanelLeft, DoublePanelRight, 200f, 200f, 9);
		}
	}
}
