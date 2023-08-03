using Settings;
using UnityEngine;

namespace UI
{
	internal class SettingsGamePanel : SettingsCategoryPanel
	{
		protected override bool CategoryPanel
		{
			get
			{
				return true;
			}
		}

		protected override string DefaultCategoryPanel
		{
			get
			{
				return "Titans";
			}
		}

		public void CreateGategoryDropdown(Transform panel, bool includeReset = true, float elementWidth = 260f)
		{
			ElementStyle style = new ElementStyle(24, 140f, ThemePanel);
			string[] options = new string[4] { "Titans", "PVP", "Misc", "Weather" };
			ElementFactory.CreateDropdownSetting(panel, style, _currentCategoryPanelName, "Category", options, "", elementWidth, 40f, 300f, null, delegate
			{
				RebuildCategoryPanel();
			});
			if (includeReset)
			{
				GameObject gameObject = ElementFactory.CreateHorizontalGroup(panel, 0f, TextAnchor.MiddleRight);
				ElementFactory.CreateDefaultButton(gameObject.transform, style, "Reset to default", 0f, 0f, delegate
				{
					OnResetButtonClick();
				});
				CreateHorizontalDivider(panel);
			}
		}

		protected void OnResetButtonClick()
		{
			SettingsManager.LegacyGameSettingsUI.SetDefault();
			RebuildCategoryPanel();
		}

		protected override void RegisterCategoryPanels()
		{
			_categoryPanelTypes.Add("Titans", typeof(SettingsGameTitansPanel));
			_categoryPanelTypes.Add("PVP", typeof(SettingsGamePVPPanel));
			_categoryPanelTypes.Add("Misc", typeof(SettingsGameMiscPanel));
			_categoryPanelTypes.Add("Weather", typeof(SettingsGameWeatherPanel));
		}
	}
}
