using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class SettingsPopup : BasePopup
	{
		public string LocaleCategory = "SettingsPopup";

		public KeybindPopup KeybindPopup;

		public ColorPickPopup ColorPickPopup;

		public SetNamePopup SetNamePopup;

		public ImportPopup ImportPopup;

		public ExportPopup ExportPopup;

		public EditWeatherSchedulePopup EditWeatherSchedulePopup;

		private List<BaseSettingsContainer> _ignoreDefaultButtonSettings = new List<BaseSettingsContainer>();

		private List<SaveableSettingsContainer> _saveableSettings = new List<SaveableSettingsContainer>();

		protected override string Title
		{
			get
			{
				return string.Empty;
			}
		}

		protected override float Width
		{
			get
			{
				return 1010f;
			}
		}

		protected override float Height
		{
			get
			{
				return 630f;
			}
		}

		protected override bool CategoryPanel
		{
			get
			{
				return true;
			}
		}

		protected override bool CategoryButtons
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
				return "General";
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			SetupBottomButtons();
			SetupSettingsList();
		}

		protected override void SetupTopButtons()
		{
			ElementStyle style = new ElementStyle(28, 120f, ThemePanel);
			string[] array = new string[8] { "General", "Graphics", "UI", "Keybinds", "Skins", "CustomMap", "Game", "Ability" };
			foreach (string buttonName in array)
			{
				GameObject gameObject = ElementFactory.CreateCategoryButton(TopBar, style, UIManager.GetLocale(LocaleCategory, "Top", buttonName + "Button"), delegate
				{
					SetCategoryPanel(buttonName);
				});
				_topButtons.Add(buttonName, gameObject.GetComponent<Button>());
			}
			base.SetupTopButtons();
		}

		protected override void RegisterCategoryPanels()
		{
			_categoryPanelTypes.Add("General", typeof(SettingsGeneralPanel));
			_categoryPanelTypes.Add("Graphics", typeof(SettingsGraphicsPanel));
			_categoryPanelTypes.Add("UI", typeof(SettingsUIPanel));
			_categoryPanelTypes.Add("Keybinds", typeof(SettingsKeybindsPanel));
			_categoryPanelTypes.Add("Skins", typeof(SettingsSkinsPanel));
			_categoryPanelTypes.Add("CustomMap", typeof(SettingsCustomMapPanel));
			_categoryPanelTypes.Add("Game", typeof(SettingsGamePanel));
			_categoryPanelTypes.Add("Ability", typeof(SettingsAbilityPanel));
		}

		protected override void SetupPopups()
		{
			base.SetupPopups();
			KeybindPopup = ElementFactory.CreateHeadedPanel<KeybindPopup>(base.transform).GetComponent<KeybindPopup>();
			ColorPickPopup = ElementFactory.CreateHeadedPanel<ColorPickPopup>(base.transform).GetComponent<ColorPickPopup>();
			SetNamePopup = ElementFactory.CreateHeadedPanel<SetNamePopup>(base.transform).GetComponent<SetNamePopup>();
			ImportPopup = ElementFactory.CreateHeadedPanel<ImportPopup>(base.transform).GetComponent<ImportPopup>();
			ExportPopup = ElementFactory.CreateHeadedPanel<ExportPopup>(base.transform).GetComponent<ExportPopup>();
			EditWeatherSchedulePopup = ElementFactory.CreateHeadedPanel<EditWeatherSchedulePopup>(base.transform).GetComponent<EditWeatherSchedulePopup>();
			_popups.Add(KeybindPopup);
			_popups.Add(ColorPickPopup);
			_popups.Add(SetNamePopup);
			_popups.Add(ImportPopup);
			_popups.Add(ExportPopup);
			_popups.Add(EditWeatherSchedulePopup);
		}

		private void SetupSettingsList()
		{
			_saveableSettings.Add(SettingsManager.GeneralSettings);
			_saveableSettings.Add(SettingsManager.GraphicsSettings);
			_saveableSettings.Add(SettingsManager.UISettings);
			_saveableSettings.Add(SettingsManager.InputSettings);
			_saveableSettings.Add(SettingsManager.CustomSkinSettings);
			_saveableSettings.Add(SettingsManager.AbilitySettings);
			_saveableSettings.Add(SettingsManager.LegacyGameSettingsUI);
			_saveableSettings.Add(SettingsManager.WeatherSettings);
			_ignoreDefaultButtonSettings.Add(SettingsManager.CustomSkinSettings);
			_ignoreDefaultButtonSettings.Add(SettingsManager.WeatherSettings);
		}

		private void SetupBottomButtons()
		{
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			string[] array = new string[5] { "Default", "Load", "Save", "Continue", "Quit" };
			foreach (string buttonName in array)
			{
				GameObject gameObject = ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon(buttonName), 0f, 0f, delegate
				{
					OnBottomBarButtonClick(buttonName);
				});
			}
		}

		private void OnConfirmSetDefault()
		{
			foreach (SaveableSettingsContainer saveableSetting in _saveableSettings)
			{
				if (!_ignoreDefaultButtonSettings.Contains(saveableSetting))
				{
					saveableSetting.SetDefault();
					saveableSetting.Save();
				}
			}
			RebuildCategoryPanel();
			UIManager.CurrentMenu.MessagePopup.Show("Settings reset to default.");
		}

		private void OnBottomBarButtonClick(string name)
		{
			switch (name)
			{
			case "Save":
				foreach (SaveableSettingsContainer saveableSetting in _saveableSettings)
				{
					saveableSetting.Save();
				}
				if (Application.loadedLevel == 0)
				{
					Hide();
				}
				else
				{
					GameMenu.TogglePause(false);
				}
				break;
			case "Load":
				foreach (SaveableSettingsContainer saveableSetting2 in _saveableSettings)
				{
					saveableSetting2.Load();
				}
				RebuildCategoryPanel();
				UIManager.CurrentMenu.MessagePopup.Show("Settings loaded from file.");
				break;
			case "Continue":
				if (Application.loadedLevel == 0)
				{
					Hide();
				}
				else
				{
					GameMenu.TogglePause(false);
				}
				break;
			case "Default":
				UIManager.CurrentMenu.ConfirmPopup.Show("Are you sure you want to reset to default?", delegate
				{
					OnConfirmSetDefault();
				}, "Reset default");
				break;
			case "Quit":
				foreach (SaveableSettingsContainer saveableSetting3 in _saveableSettings)
				{
					saveableSetting3.Load();
				}
				if (Application.loadedLevel == 0)
				{
					Application.Quit();
					break;
				}
				GameMenu.TogglePause(false);
				if (PhotonNetwork.connected)
				{
					PhotonNetwork.Disconnect();
				}
				IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
				FengGameManagerMKII.instance.gameStart = false;
				FengGameManagerMKII.instance.DestroyAllExistingCloths();
				Object.Destroy(GameObject.Find("MultiplayerManager"));
				Application.LoadLevel("menu");
				break;
			}
		}

		public override void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				foreach (SaveableSettingsContainer saveableSetting in _saveableSettings)
				{
					saveableSetting.Apply();
				}
			}
			base.Hide();
		}
	}
}
