using Settings;

namespace UI
{
	internal class SettingsGamePVPPanel : SettingsCategoryPanel
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
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.PointModeEnabled, "Point mode", "End game after player or team reaches certain number of points.");
			ElementFactory.CreateInputSetting(DoublePanelLeft, style, legacyGameSettingsUI.PointModeAmount, "Point amount", "", elementWidth);
			CreateHorizontalDivider(DoublePanelLeft);
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.BombModeEnabled, "Bomb mode");
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.BombModeCeiling, "Bomb ceiling");
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.BombModeInfiniteGas, "Bomb infinite gas");
			ElementFactory.CreateToggleSetting(DoublePanelLeft, style, legacyGameSettingsUI.BombModeDisableTitans, "Bomb disable titans");
			ElementFactory.CreateToggleGroupSetting(DoublePanelRight, style, legacyGameSettingsUI.TeamMode, "Team mode", new string[4] { "Off", "No sort", "Size lock", "Skill lock" });
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.InfectionModeEnabled, "Infection mode");
			ElementFactory.CreateInputSetting(DoublePanelRight, style, legacyGameSettingsUI.InfectionModeAmount, "Starting titans", "", elementWidth);
			CreateHorizontalDivider(DoublePanelRight);
			ElementFactory.CreateToggleGroupSetting(DoublePanelRight, style, legacyGameSettingsUI.BladePVP, "Blade/AHSS PVP", new string[3] { "Off", "Teams", "FFA" });
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.FriendlyMode, "Friendly mode", "Prevent normal AHSS/Blade PVP.");
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.AHSSAirReload, "AHSS air reload");
			ElementFactory.CreateToggleSetting(DoublePanelRight, style, legacyGameSettingsUI.CannonsFriendlyFire, "Cannons friendly fire");
		}
	}
}
