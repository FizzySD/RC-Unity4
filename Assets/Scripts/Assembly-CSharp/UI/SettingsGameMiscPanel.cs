using Settings;

namespace UI
{
	internal class SettingsGameMiscPanel : SettingsCategoryPanel
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
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanPerWavesEnabled, "Custom titans/wave");
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanPerWaves, "Titan amount", "", elementWidth);
			CreateHorizontalDivider(DoublePanelLeft);
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanMaxWavesEnabled, "Custom max waves");
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.TitanMaxWaves, "Wave amount", "", elementWidth);
			CreateHorizontalDivider(DoublePanelLeft);
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.EndlessRespawnEnabled, "Endless respawn");
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.EndlessRespawnTime, "Respawn time", "", elementWidth);
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.PunksEveryFive, "Punks every 5 waves");
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.GlobalMinimapDisable, "Global minimap disable");
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.PreserveKDR, "Preserve KDR", "Preserve player stats when they leave and rejoin the room.");
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.RacingEndless, "Endless racing", "Racing round continues even if someone finishes.");
			ElementFactory.CreateInputSetting(DoublePanelRight, style, legacyGameSettingsUI.RacingStartTime, "Racing start time", "", elementWidth);
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.KickShifters, "Kick shifters");
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.AllowHorses, "Allow horses");
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.GlobalHideNames, "Global hide names");
			ElementFactory.CreateInputSetting(DoublePanelRight, new ElementStyle(24, 160f, ThemePanel), legacyGameSettingsUI.Motd, "MOTD", "", 200f);
		}
	}
}
