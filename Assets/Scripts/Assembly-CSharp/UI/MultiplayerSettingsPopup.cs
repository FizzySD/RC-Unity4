using Settings;
using UnityEngine;

namespace UI
{
	internal class MultiplayerSettingsPopup : PromptPopup
	{
		protected override string Title
		{
			get
			{
				return UIManager.GetLocale("MainMenu", "MultiplayerSettingsPopup", "Title");
			}
		}

		protected override float Width
		{
			get
			{
				return 480f;
			}
		}

		protected override float Height
		{
			get
			{
				return 550f;
			}
		}

		protected override bool DoublePanel
		{
			get
			{
				return false;
			}
		}

		protected override TextAnchor PanelAlignment
		{
			get
			{
				return TextAnchor.MiddleCenter;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			string category = "MainMenu";
			string subCategory = "MultiplayerSettingsPopup";
			MultiplayerSettings multiplayerSettings = SettingsManager.MultiplayerSettings;
			float elementWidth = 180f;
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			ElementStyle style2 = new ElementStyle(24, 160f, ThemePanel);
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Save"), 0f, 0f, delegate
			{
				OnSaveButtonClick();
			});
			ElementFactory.CreateToggleGroupSetting(SinglePanel, style2, multiplayerSettings.LobbyMode, UIManager.GetLocale(category, subCategory, "Lobby"), UIManager.GetLocaleArray(category, subCategory, "LobbyOptions"), UIManager.GetLocale(category, subCategory, "LobbyTooltip"));
			ElementFactory.CreateInputSetting(SinglePanel, style2, multiplayerSettings.CustomAppId, UIManager.GetLocale(category, subCategory, "LobbyCustom"), "", elementWidth);
			CreateHorizontalDivider(SinglePanel);
			ElementFactory.CreateToggleGroupSetting(SinglePanel, style2, multiplayerSettings.AppIdMode, UIManager.GetLocale(category, subCategory, "AppId"), UIManager.GetLocaleArray(category, subCategory, "AppIdOptions"), UIManager.GetLocale(category, subCategory, "AppIdTooltip"));
			ElementFactory.CreateInputSetting(SinglePanel, style2, multiplayerSettings.CustomAppId, UIManager.GetLocale(category, subCategory, "AppIdCustom"), "", elementWidth);
		}

		protected void OnSaveButtonClick()
		{
			SettingsManager.MultiplayerSettings.Save();
			Hide();
		}
	}
}
