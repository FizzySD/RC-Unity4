using Settings;
using UnityEngine;

namespace UI
{
	internal class MultiplayerLanPopup : PromptPopup
	{
		protected override string Title
		{
			get
			{
				return "LAN";
			}
		}

		protected override float Width
		{
			get
			{
				return 400f;
			}
		}

		protected override float Height
		{
			get
			{
				return 370f;
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
			string subCategory = "MultiplayerLanPopup";
			MultiplayerSettings multiplayerSettings = SettingsManager.MultiplayerSettings;
			float elementWidth = 200f;
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			ElementStyle style2 = new ElementStyle(24, 120f, ThemePanel);
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocale(category, subCategory, "Connect"), 0f, 0f, delegate
			{
				OnButtonClick("Connect");
			});
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Back"), 0f, 0f, delegate
			{
				OnButtonClick("Back");
			});
			ElementFactory.CreateInputSetting(SinglePanel, style2, multiplayerSettings.LanIP, "IP", "", elementWidth);
			ElementFactory.CreateInputSetting(SinglePanel, style2, multiplayerSettings.LanPort, "Port", "", elementWidth);
			ElementFactory.CreateInputSetting(SinglePanel, style2, multiplayerSettings.LanPassword, "Password (optional)", "", elementWidth);
		}

		protected void OnButtonClick(string name)
		{
			if (name == "Connect")
			{
				SettingsManager.MultiplayerSettings.ConnectLAN();
			}
			else if (name == "Back")
			{
				Hide();
			}
		}
	}
}
