using Settings;
using UnityEngine;

namespace UI
{
	internal class SettingsCustomMapPanel : SettingsCategoryPanel
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
			SettingsPopup settingsPopup = (SettingsPopup)parent;
			string localeCategory = settingsPopup.LocaleCategory;
			string text = "CustomMap";
			LegacyGameSettings legacyGameSettingsUI = SettingsManager.LegacyGameSettingsUI;
			ElementStyle style = new ElementStyle(24, 200f, ThemePanel);
			ElementStyle style2 = new ElementStyle(24, 120f, ThemePanel);
			ElementStyle style3 = new ElementStyle(28, 120f, ThemePanel);
			ElementFactory.CreateDefaultLabel(DoublePanelLeft, style, "Map script");
			ElementFactory.CreateInputSetting(DoublePanelLeft, style2, legacyGameSettingsUI.LevelScript, string.Empty, "", 420f, 300f, true);
			GameObject gameObject = ElementFactory.CreateHorizontalGroup(DoublePanelLeft, 0f, TextAnchor.UpperCenter);
			ElementFactory.CreateDefaultButton(gameObject.transform, style3, "Clear", 0f, 0f, delegate
			{
				OnCustomMapButtonClick("ClearMap");
			});
			string[] options = new string[5] { "Survive", "Waves", "PVP", "Racing", "Custom" };
			ElementFactory.CreateDropdownSetting(DoublePanelRight, style, legacyGameSettingsUI.GameType, "Game mode", options);
			ElementFactory.CreateInputSetting(DoublePanelRight, style, legacyGameSettingsUI.TitanSpawnCap, "Titan cap");
			CreateHorizontalDivider(DoublePanelRight);
			ElementFactory.CreateDefaultLabel(DoublePanelRight, style, "Logic script");
			ElementFactory.CreateInputSetting(DoublePanelRight, style2, legacyGameSettingsUI.LogicScript, string.Empty, "", 420f, 300f, true);
			gameObject = ElementFactory.CreateHorizontalGroup(DoublePanelRight, 0f, TextAnchor.UpperCenter);
			ElementFactory.CreateDefaultButton(gameObject.transform, style3, "Clear", 0f, 0f, delegate
			{
				OnCustomMapButtonClick("ClearLogic");
			});
		}

		private void OnCustomMapButtonClick(string name)
		{
			if (name == "ClearMap")
			{
				SettingsManager.LegacyGameSettingsUI.LevelScript.Value = string.Empty;
			}
			else if (name == "ClearLogic")
			{
				SettingsManager.LegacyGameSettingsUI.LogicScript.Value = string.Empty;
			}
			Parent.RebuildCategoryPanel();
		}
	}
}
