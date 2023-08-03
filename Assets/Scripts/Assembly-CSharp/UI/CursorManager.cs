using System;
using System.Collections.Generic;
using ApplicationManagers;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
	internal class CursorManager : MonoBehaviour
	{
		public static CursorState State;

		private static CursorManager _instance;

		private static Texture2D _cursorPointer;

		private static Dictionary<CrosshairStyle, Texture2D> _crosshairs = new Dictionary<CrosshairStyle, Texture2D>();

		private bool _ready;

		private bool _crosshairWhite = true;

		private bool _lastCrosshairWhite;

		private string _crosshairText = string.Empty;

		private bool _forceNextCrosshairUpdate;

		private CrosshairStyle _lastCrosshairStyle;

		public static void Init()
		{
			_instance = SingletonFactory.CreateSingleton(_instance);
		}

		public static void FinishLoadAssets()
		{
			_cursorPointer = (Texture2D)AssetBundleManager.MainAssetBundle.Load("CursorPointer");
			foreach (CrosshairStyle value2 in Enum.GetValues(typeof(CrosshairStyle)))
			{
				Texture2D value = (Texture2D)AssetBundleManager.MainAssetBundle.Load("Cursor" + value2);
				_crosshairs.Add(value2, value);
			}
			_instance._ready = true;
			SetPointer(true);
		}

		private void Update()
		{
			if (Application.loadedLevel == 0 || Application.loadedLevelName == "characterCreation" || Application.loadedLevelName == "Snapshot")
			{
				SetPointer();
			}
			else if (Application.loadedLevel == 2 && (int)FengGameManagerMKII.settingsOld[64] >= 100)
			{
				if (Camera.main.GetComponent<MouseLook>().enabled)
				{
					SetHidden();
				}
				else
				{
					SetPointer();
				}
			}
			else if (GameMenu.InMenu() || IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.STOP)
			{
				SetPointer();
			}
			else if (!FengGameManagerMKII.logicLoaded || !FengGameManagerMKII.customLevelLoaded)
			{
				SetPointer();
			}
			else if (FengGameManagerMKII.instance.needChooseSide && NGUITools.GetActive(FengGameManagerMKII.instance.ui.GetComponent<UIReferArray>().panels[3]))
			{
				SetPointer();
			}
			else if (IN_GAME_MAIN_CAMERA.Instance.main_object != null)
			{
				GameObject main_object = IN_GAME_MAIN_CAMERA.Instance.main_object;
				HERO component = main_object.GetComponent<HERO>();
				if (SettingsManager.LegacyGeneralSettings.SpecMode.Value || component == null || !component.IsMine())
				{
					SetHidden();
				}
				else
				{
					SetCrosshair();
				}
			}
			else
			{
				SetHidden();
			}
		}

		public static void RefreshCursorLock()
		{
			if (Screen.lockCursor)
			{
				Screen.lockCursor = !Screen.lockCursor;
				Screen.lockCursor = !Screen.lockCursor;
			}
		}

		public static void SetPointer(bool force = false)
		{
			if (force || State != 0)
			{
				Screen.showCursor = true;
				Screen.lockCursor = false;
				State = CursorState.Pointer;
			}
		}

		public static void SetHidden(bool force = false)
		{
			if (force || State != CursorState.Hidden)
			{
				Screen.showCursor = false;
				State = CursorState.Hidden;
			}
			if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
			{
				if (!Screen.lockCursor)
				{
					Screen.lockCursor = true;
				}
			}
			else if (Screen.lockCursor)
			{
				Screen.lockCursor = false;
			}
		}

		public static void SetCrosshair(bool force = false)
		{
			if (force || State != CursorState.Crosshair)
			{
				Screen.showCursor = false;
				State = CursorState.Crosshair;
			}
			if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
			{
				if (!Screen.lockCursor)
				{
					Screen.lockCursor = true;
				}
			}
			else if (Screen.lockCursor)
			{
				Screen.lockCursor = false;
			}
		}

		public static void SetCrosshairColor(bool white)
		{
			if (_instance._crosshairWhite != white)
			{
				_instance._crosshairWhite = white;
			}
		}

		public static void SetCrosshairText(string text)
		{
			_instance._crosshairText = text;
		}

		public static void UpdateCrosshair(RawImage crosshairImageWhite, RawImage crosshairImageRed, Text crosshairLabelWhite, Text crosshairLabelRed, bool force = false)
		{
			if (!_instance._ready)
			{
				return;
			}
			if (State != CursorState.Crosshair || GameMenu.HideCrosshair)
			{
				if (crosshairImageRed.gameObject.activeSelf)
				{
					crosshairImageRed.gameObject.SetActive(false);
				}
				if (crosshairImageWhite.gameObject.activeSelf)
				{
					crosshairImageWhite.gameObject.SetActive(false);
				}
				_instance._forceNextCrosshairUpdate = true;
				return;
			}
			CrosshairStyle value = (CrosshairStyle)SettingsManager.UISettings.CrosshairStyle.Value;
			if (_instance._lastCrosshairStyle != value || force || _instance._forceNextCrosshairUpdate)
			{
				crosshairImageWhite.texture = _crosshairs[value];
				crosshairImageRed.texture = _crosshairs[value];
				_instance._lastCrosshairStyle = value;
			}
			if (_instance._crosshairWhite != _instance._lastCrosshairWhite || force || _instance._forceNextCrosshairUpdate)
			{
				crosshairImageWhite.gameObject.SetActive(_instance._crosshairWhite);
				crosshairImageRed.gameObject.SetActive(!_instance._crosshairWhite);
				_instance._lastCrosshairWhite = _instance._crosshairWhite;
			}
			Text text = crosshairLabelWhite;
			RawImage rawImage = crosshairImageWhite;
			if (!_instance._crosshairWhite)
			{
				text = crosshairLabelRed;
				rawImage = crosshairImageRed;
			}
			text.text = _instance._crosshairText;
			Vector3 mousePosition = Input.mousePosition;
			Transform transform = rawImage.transform;
			if (transform.position != mousePosition)
			{
				if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
				{
					if (Math.Abs(transform.position.x - mousePosition.x) > 1f || Math.Abs(transform.position.y - mousePosition.y) > 1f)
					{
						transform.position = mousePosition;
					}
				}
				else
				{
					transform.position = mousePosition;
				}
			}
			_instance._forceNextCrosshairUpdate = false;
		}
	}
}
