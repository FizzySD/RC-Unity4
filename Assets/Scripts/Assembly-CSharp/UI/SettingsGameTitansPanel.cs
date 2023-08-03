using Settings;

namespace UI
{
	internal class SettingsGameTitansPanel : SettingsCategoryPanel
	{
		protected override bool ScrollBar
		{
			get
			{
				return true;
			}
		}

		protected override float VerticalSpacing
		{
			get
			{
				return 20f;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			SettingsGamePanel settingsGamePanel = (SettingsGamePanel)parent;
			SettingsPopup settingsPopup = (SettingsPopup)settingsGamePanel.Parent;
			settingsGamePanel.CreateGategoryDropdown(DoublePanelLeft);
			float elementWidth = 120f;
			ElementStyle style = new ElementStyle(24, 240f, ThemePanel);
			LegacyGameSettings legacyGameSettingsUI = SettingsManager.LegacyGameSettingsUI;
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanNumberEnabled, "Custom titan number");
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanNumber, "Titan amount", "", elementWidth);
			CreateHorizontalDivider(DoublePanelLeft);
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSpawnEnabled, "Custom titan spawns", "Spawn rates must add up to 100.");
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSpawnNormal, "Normal", "", elementWidth);
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSpawnAberrant, "Aberrant", "", elementWidth);
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSpawnJumper, "Jumper", "", elementWidth);
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSpawnCrawler, "Crawler", "", elementWidth);
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSpawnPunk, "Punk", "", elementWidth);
			CreateHorizontalDivider(DoublePanelLeft);
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSizeEnabled, "Custom titan sizes");
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSizeMin, "Minimum size", "", elementWidth);
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanSizeMax, "Maximum size", "", elementWidth);
			ElementFactory.CreateToggleGroupSetting(DoublePanelRight, style, legacyGameSettingsUI.TitanHealthMode, "Titan health", new string[3] { "Off", "Fixed", "Scaled" });
			ElementFactory.CreateInputSetting(DoublePanelRight, style, legacyGameSettingsUI.TitanHealthMin, "Minimum health", "", elementWidth);
			ElementFactory.CreateInputSetting(DoublePanelRight, style, legacyGameSettingsUI.TitanHealthMax, "Maximum health", "", elementWidth);
			CreateHorizontalDivider(DoublePanelRight);
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.TitanArmorEnabled, "Titan armor");
			ElementFactory.CreateInputSetting(DoublePanelRight, style, legacyGameSettingsUI.TitanArmor, "Armor amount", "", elementWidth);
			CreateHorizontalDivider(DoublePanelRight);
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.TitanExplodeEnabled, "Titan explode");
			ElementFactory.CreateInputSetting(DoublePanelRight, style, legacyGameSettingsUI.TitanExplodeRadius, "Explode radius", "", elementWidth);
			CreateHorizontalDivider(DoublePanelRight);
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.RockThrowEnabled, "Punk rock throwing");
		}
	}
}
