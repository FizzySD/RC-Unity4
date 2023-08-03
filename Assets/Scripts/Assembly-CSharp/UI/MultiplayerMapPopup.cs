using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class MultiplayerMapPopup : BasePopup
	{
		protected MultiplayerSettingsPopup _multiplayerSettingsPopup;

		protected MultiplayerLanPopup _lanPopup;

		protected override string ThemePanel
		{
			get
			{
				return "MultiplayerMapPopup";
			}
		}

		protected override int HorizontalPadding
		{
			get
			{
				return 0;
			}
		}

		protected override int VerticalPadding
		{
			get
			{
				return 0;
			}
		}

		protected override float VerticalSpacing
		{
			get
			{
				return 0f;
			}
		}

		protected override string Title
		{
			get
			{
				return UIManager.GetLocale("MainMenu", "MultiplayerMapPopup", "Title");
			}
		}

		protected override bool HasPremadeContent
		{
			get
			{
				return true;
			}
		}

		protected override float Width
		{
			get
			{
				return 900f;
			}
		}

		protected override float Height
		{
			get
			{
				return 560f;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			ElementStyle elementStyle = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			GameObject gameObject = SinglePanel.Find("MultiplayerMap").gameObject;
			Button[] componentsInChildren = gameObject.GetComponentsInChildren<Button>();
			foreach (Button button in componentsInChildren)
			{
				button.onClick.AddListener(delegate
				{
					OnButtonClick(button.name);
				});
				button.GetComponent<Button>().colors = UIManager.GetThemeColorBlock(elementStyle.ThemePanel, "DefaultButton", "");
				button.transform.Find("Text").GetComponent<Text>().color = UIManager.GetThemeColor(elementStyle.ThemePanel, "DefaultButton", "TextColor");
			}
			string category = "MainMenu";
			string subCategory = "MultiplayerMapPopup";
			ElementFactory.CreateDefaultButton(BottomBar, elementStyle, "LAN", 0f, 0f, delegate
			{
				OnButtonClick("LAN");
			});
			ElementFactory.CreateDefaultButton(BottomBar, elementStyle, UIManager.GetLocale(category, subCategory, "ButtonOffline"), 0f, 0f, delegate
			{
				OnButtonClick("Offline");
			});
			ElementFactory.CreateDefaultButton(BottomBar, elementStyle, UIManager.GetLocale(category, subCategory, "ButtonServer"), 0f, 0f, delegate
			{
				OnButtonClick("Server");
			});
			ElementFactory.CreateDefaultButton(BottomBar, elementStyle, UIManager.GetLocaleCommon("Back"), 0f, 0f, delegate
			{
				OnButtonClick("Back");
			});
			gameObject.GetComponent<Image>().color = UIManager.GetThemeColor(elementStyle.ThemePanel, "MainBody", "MapColor");
		}

		protected override void SetupPopups()
		{
			base.SetupPopups();
			_multiplayerSettingsPopup = ElementFactory.CreateHeadedPanel<MultiplayerSettingsPopup>(base.transform).GetComponent<MultiplayerSettingsPopup>();
			_lanPopup = ElementFactory.CreateHeadedPanel<MultiplayerLanPopup>(base.transform).GetComponent<MultiplayerLanPopup>();
			_popups.Add(_multiplayerSettingsPopup);
			_popups.Add(_lanPopup);
		}

		private void OnButtonClick(string name)
		{
			HideAllPopups();
			MultiplayerSettings multiplayerSettings = SettingsManager.MultiplayerSettings;
			switch (name)
			{
			case "Back":
				Hide();
				break;
			case "Server":
				_multiplayerSettingsPopup.Show();
				break;
			case "Offline":
				multiplayerSettings.ConnectOffline();
				break;
			case "LAN":
				_lanPopup.Show();
				break;
			case "ButtonUS":
				multiplayerSettings.ConnectServer(MultiplayerRegion.US);
				break;
			case "ButtonSA":
				multiplayerSettings.ConnectServer(MultiplayerRegion.SA);
				break;
			case "ButtonEU":
				multiplayerSettings.ConnectServer(MultiplayerRegion.EU);
				break;
			case "ButtonASIA":
				multiplayerSettings.ConnectServer(MultiplayerRegion.ASIA);
				break;
			}
		}
	}
}
