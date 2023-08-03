using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace UI
{
	internal class MultiplayerCreatePopup : PromptPopup
	{
		protected override string Title
		{
			get
			{
				return UIManager.GetLocale("MainMenu", "MultiplayerCreatePopup", "Title");
			}
		}

		protected override float Width
		{
			get
			{
				return 800f;
			}
		}

		protected override float Height
		{
			get
			{
				return 500f;
			}
		}

		protected override bool DoublePanel
		{
			get
			{
				return true;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			MultiplayerGameSettings multiplayerGameSettings = SettingsManager.MultiplayerGameSettings;
			string category = "MainMenu";
			string subCategory = "MultiplayerCreatePopup";
			string subCategory2 = "SingleplayerPopup";
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			ElementStyle style2 = new ElementStyle(24, 120f, ThemePanel);
			ElementStyle style3 = new ElementStyle(24, 200f, ThemePanel);
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Start"), 0f, 0f, delegate
			{
				OnButtonClick("Start");
			});
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Back"), 0f, 0f, delegate
			{
				OnButtonClick("Back");
			});
			ElementFactory.CreateDropdownSetting(DoublePanelLeft, style2, multiplayerGameSettings.Map, UIManager.GetLocaleCommon("Map"), GetMapOptions(), "", 180f, 40f, 300f, 360f);
			ElementFactory.CreateDropdownSetting(DoublePanelLeft, style2, SettingsManager.WeatherSettings.WeatherSets.GetSelectedSetIndex(), UIManager.GetLocale(category, subCategory2, "Weather"), SettingsManager.WeatherSettings.WeatherSets.GetSetNames(), "", 180f);
			ElementFactory.CreateToggleGroupSetting(DoublePanelLeft, style2, multiplayerGameSettings.Difficulty, UIManager.GetLocale(category, subCategory2, "Difficulty"), UIManager.GetLocaleArray(category, subCategory2, "DifficultyOptions"));
			ElementFactory.CreateInputSetting(DoublePanelRight, style3, multiplayerGameSettings.Name, UIManager.GetLocale(category, subCategory, "ServerName"), "", 200f);
			ElementFactory.CreateInputSetting(DoublePanelRight, style3, multiplayerGameSettings.Password, UIManager.GetLocaleCommon("Password"), "", 200f);
			ElementFactory.CreateInputSetting(DoublePanelRight, style3, multiplayerGameSettings.MaxPlayers, UIManager.GetLocale(category, subCategory, "MaxPlayers"), "", 200f);
			ElementFactory.CreateInputSetting(DoublePanelRight, style3, multiplayerGameSettings.MaxTime, UIManager.GetLocale(category, subCategory, "MaxTime"), "", 200f);
		}

		private string[] GetMapOptions()
		{
			LevelInfo.Init();
			int[] array = new int[18]
			{
				0, 3, 4, 5, 17, 6, 7, 8, 9, 10,
				11, 19, 20, 21, 22, 23, 25, 26
			};
			List<string> list = new List<string>();
			int[] array2 = array;
			foreach (int num in array2)
			{
				list.Add(LevelInfo.levels[num].name);
			}
			return list.ToArray();
		}

		private void OnButtonClick(string name)
		{
			if (name == "Back")
			{
				Hide();
			}
			else if (name == "Start")
			{
				StartMultiplayer();
			}
		}

		private void StartMultiplayer()
		{
			MultiplayerGameSettings multiplayerGameSettings = SettingsManager.MultiplayerGameSettings;
			string value = multiplayerGameSettings.Name.Value;
			int value2 = multiplayerGameSettings.MaxPlayers.Value;
			int value3 = multiplayerGameSettings.MaxTime.Value;
			string value4 = multiplayerGameSettings.Map.Value;
			string text = "normal";
			if (multiplayerGameSettings.Difficulty.Value == 2)
			{
				text = "abnormal";
			}
			else if (multiplayerGameSettings.Difficulty.Value == 1)
			{
				text = "hard";
			}
			string text2 = "day";
			string text3 = multiplayerGameSettings.Password.Value;
			if (text3.Length > 0)
			{
				text3 = new SimpleAES().Encrypt(text3);
			}
			PhotonNetwork.CreateRoom(value + "`" + value4 + "`" + text + "`" + value3 + "`" + text2 + "`" + text3 + "`" + Random.Range(0, 50000), true, true, value2);
		}
	}
}
