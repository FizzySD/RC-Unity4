using System.Collections.Generic;
using GameManagers;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class GameMenu : BaseMenu
	{
		public static Dictionary<string, Texture2D> EmojiTextures = new Dictionary<string, Texture2D>();

		public static List<string> AvailableEmojis = new List<string> { "Smile", "ThumbsUp", "Cool", "Love", "Shocked", "Crying", "Annoyed", "Angry" };

		public static List<string> AvailableText = new List<string> { "Help", "Thanks", "Sorry", "Titan here", "Good game", "Nice hit", "Oops", "Welcome" };

		public static List<string> AvailableActions = new List<string> { "Salute", "Dance", "Flip", "Wave1", "Wave2", "Eat" };

		private const float EmoteCooldown = 4f;

		public static bool Paused;

		public static bool WheelMenu;

		public static bool HideCrosshair;

		public List<BasePopup> _emoteTextPopups = new List<BasePopup>();

		public List<BasePopup> _emoteEmojiPopups = new List<BasePopup>();

		public BasePopup _settingsPopup;

		public BasePopup _emoteWheelPopup;

		public BasePopup _itemWheelPopup;

		public RawImage _crosshairImageWhite;

		public RawImage _crosshairImageRed;

		public Text _crosshairLabelWhite;

		public Text _crosshairLabelRed;

		private float _currentEmoteCooldown;

		private EmoteWheelState _currentEmoteWheelState;

		public override void Setup()
		{
			base.Setup();
			HideCrosshair = false;
			TogglePause(false);
			WheelMenu = false;
			SetupCrosshairs();
		}

		public static bool InMenu()
		{
			if (!Paused)
			{
				return WheelMenu;
			}
			return true;
		}

		public static void TogglePause(bool pause)
		{
			Paused = pause;
			if (UIManager.CurrentMenu != null && UIManager.CurrentMenu.GetComponent<GameMenu>() != null)
			{
				GameMenu component = UIManager.CurrentMenu.GetComponent<GameMenu>();
				if (Paused && !component._settingsPopup.gameObject.activeSelf)
				{
					component._settingsPopup.Show();
					component._emoteWheelPopup.Hide();
					WheelMenu = false;
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
					{
						Time.timeScale = 0f;
					}
				}
				else
				{
					Paused = false;
					component._settingsPopup.Hide();
					if (!Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled)
					{
						Camera.main.GetComponent<SpectatorMovement>().disable = false;
						Camera.main.GetComponent<MouseLook>().disable = false;
					}
				}
			}
			if (!Paused && FengGameManagerMKII.instance.pauseWaitTime <= 0f)
			{
				Time.timeScale = 1f;
			}
		}

		public static void OnEmoteTextRPC(int viewId, string text, PhotonMessageInfo info)
		{
			if (!(UIManager.CurrentMenu == null) && SettingsManager.UISettings.ShowEmotes.Value)
			{
				GameMenu component = UIManager.CurrentMenu.GetComponent<GameMenu>();
				Transform transformFromViewId = GetTransformFromViewId(viewId, info);
				if (transformFromViewId != null && component != null)
				{
					component.ShowEmoteText(text, transformFromViewId);
				}
			}
		}

		public static void OnEmoteEmojiRPC(int viewId, string emoji, PhotonMessageInfo info)
		{
			if (!(UIManager.CurrentMenu == null) && SettingsManager.UISettings.ShowEmotes.Value)
			{
				GameMenu component = UIManager.CurrentMenu.GetComponent<GameMenu>();
				Transform transformFromViewId = GetTransformFromViewId(viewId, info);
				if (transformFromViewId != null && component != null)
				{
					component.ShowEmoteEmoji(emoji, transformFromViewId);
				}
			}
		}

		public static void ToggleEmoteWheel(bool enable)
		{
			if (!(UIManager.CurrentMenu != null) || !(UIManager.CurrentMenu.GetComponent<GameMenu>() != null))
			{
				return;
			}
			GameMenu menu = UIManager.CurrentMenu.GetComponent<GameMenu>();
			if (enable)
			{
				((WheelPopup)menu._emoteWheelPopup).Show(SettingsManager.InputSettings.Interaction.EmoteMenu.ToString(), GetEmoteWheelOptions(menu._currentEmoteWheelState), delegate
				{
					menu.OnEmoteWheelSelect();
				});
				WheelMenu = true;
			}
			else
			{
				menu._emoteWheelPopup.Hide();
				WheelMenu = false;
			}
		}

		public static void NextEmoteWheel()
		{
			if (!(UIManager.CurrentMenu != null) || !(UIManager.CurrentMenu.GetComponent<GameMenu>() != null))
			{
				return;
			}
			GameMenu menu = UIManager.CurrentMenu.GetComponent<GameMenu>();
			if (menu._emoteWheelPopup.gameObject.activeSelf && WheelMenu)
			{
				menu._currentEmoteWheelState++;
				if (menu._currentEmoteWheelState > EmoteWheelState.Action)
				{
					menu._currentEmoteWheelState = EmoteWheelState.Text;
				}
				((WheelPopup)menu._emoteWheelPopup).Show(SettingsManager.InputSettings.Interaction.EmoteMenu.ToString(), GetEmoteWheelOptions(menu._currentEmoteWheelState), delegate
				{
					menu.OnEmoteWheelSelect();
				});
			}
		}

		public void ShowEmoteText(string text, Transform parent)
		{
			EmoteTextPopup emoteTextPopup = (EmoteTextPopup)GetAvailablePopup(_emoteTextPopups);
			if (text.Length > 20)
			{
				text = text.Substring(0, 20);
			}
			emoteTextPopup.Show(text, parent);
		}

		public void ShowEmoteEmoji(string emoji, Transform parent)
		{
			EmoteEmojiPopup emoteEmojiPopup = (EmoteEmojiPopup)GetAvailablePopup(_emoteEmojiPopups);
			emoteEmojiPopup.Show(emoji, parent);
		}

		private void OnEmoteWheelSelect()
		{
			if (_currentEmoteWheelState != EmoteWheelState.Action)
			{
				if (_currentEmoteCooldown > 0f)
				{
					return;
				}
				_currentEmoteCooldown = 4f;
			}
			HERO myHero = RCextensions.GetMyHero();
			if (myHero == null)
			{
				return;
			}
			if (_currentEmoteWheelState == EmoteWheelState.Text)
			{
				string text = AvailableText[((WheelPopup)_emoteWheelPopup).SelectedItem];
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					ShowEmoteText(text, myHero.transform);
				}
				else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					CustomRPCManager.PhotonView.RPC("EmoteTextRPC", PhotonTargets.All, myHero.photonView.viewID, text);
				}
			}
			else if (_currentEmoteWheelState == EmoteWheelState.Emoji)
			{
				string text2 = AvailableEmojis[((WheelPopup)_emoteWheelPopup).SelectedItem];
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					ShowEmoteEmoji(text2, myHero.transform);
				}
				else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					CustomRPCManager.PhotonView.RPC("EmoteEmojiRPC", PhotonTargets.All, myHero.photonView.viewID, text2);
				}
			}
			else if (_currentEmoteWheelState == EmoteWheelState.Action)
			{
				switch (AvailableActions[((WheelPopup)_emoteWheelPopup).SelectedItem])
				{
				case "Salute":
					myHero.EmoteAction("salute");
					break;
				case "Dance":
					myHero.EmoteAction("special_armin");
					break;
				case "Flip":
					myHero.EmoteAction("dodge");
					break;
				case "Wave1":
					myHero.EmoteAction("special_marco_0");
					break;
				case "Wave2":
					myHero.EmoteAction("special_marco_1");
					break;
				case "Eat":
					myHero.EmoteAction("special_sasha");
					break;
				}
			}
			myHero._flareDelayAfterEmote = 2f;
			_emoteWheelPopup.Hide();
			WheelMenu = false;
		}

		private static Transform GetTransformFromViewId(int viewId, PhotonMessageInfo info)
		{
			PhotonView photonView = PhotonView.Find(viewId);
			if (photonView != null && photonView.owner == info.sender)
			{
				return photonView.transform;
			}
			return null;
		}

		private static List<string> GetEmoteWheelOptions(EmoteWheelState state)
		{
			switch (state)
			{
			case EmoteWheelState.Text:
				return AvailableText;
			case EmoteWheelState.Emoji:
				return AvailableEmojis;
			default:
				return AvailableActions;
			}
		}

		private BasePopup GetAvailablePopup(List<BasePopup> popups)
		{
			foreach (BasePopup popup in popups)
			{
				if (!popup.gameObject.activeSelf)
				{
					return popup;
				}
			}
			return popups[0];
		}

		protected void SetupCrosshairs()
		{
			_crosshairImageWhite = ElementFactory.InstantiateAndBind(base.transform, "CrosshairImage").GetComponent<RawImage>();
			_crosshairImageRed = ElementFactory.InstantiateAndBind(base.transform, "CrosshairImage").GetComponent<RawImage>();
			_crosshairImageRed.color = Color.red;
			_crosshairLabelWhite = _crosshairImageWhite.transform.Find("DefaultLabel").GetComponent<Text>();
			_crosshairLabelRed = _crosshairImageRed.transform.Find("DefaultLabel").GetComponent<Text>();
			ElementFactory.SetAnchor(_crosshairImageWhite.gameObject, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, Vector2.zero);
			ElementFactory.SetAnchor(_crosshairImageRed.gameObject, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, Vector2.zero);
			_crosshairImageWhite.gameObject.AddComponent<CrosshairScaler>();
			_crosshairImageRed.gameObject.AddComponent<CrosshairScaler>();
			CursorManager.UpdateCrosshair(_crosshairImageWhite, _crosshairImageRed, _crosshairLabelWhite, _crosshairLabelRed, true);
		}

		protected override void SetupPopups()
		{
			base.SetupPopups();
			_settingsPopup = ElementFactory.CreateHeadedPanel<SettingsPopup>(base.transform).GetComponent<BasePopup>();
			_emoteWheelPopup = ElementFactory.InstantiateAndSetupPanel<WheelPopup>(base.transform, "WheelMenu").GetComponent<BasePopup>();
			for (int i = 0; i < 5; i++)
			{
				BasePopup component = ElementFactory.InstantiateAndSetupPanel<EmoteTextPopup>(base.transform, "EmoteTextPopup").GetComponent<BasePopup>();
				_emoteTextPopups.Add(component);
				BasePopup component2 = ElementFactory.InstantiateAndSetupPanel<EmoteEmojiPopup>(base.transform, "EmoteEmojiPopup").GetComponent<BasePopup>();
				_emoteEmojiPopups.Add(component2);
			}
			_popups.Add(_settingsPopup);
			_popups.Add(_emoteWheelPopup);
		}

		private void Update()
		{
			CursorManager.UpdateCrosshair(_crosshairImageWhite, _crosshairImageRed, _crosshairLabelWhite, _crosshairLabelRed);
			_currentEmoteCooldown -= Time.deltaTime;
		}
	}
}
