using CustomSkins;
using GameProgress;
using Map;
using Settings;
using UI;
using UnityEngine;
using Utility;
using Weather;

namespace ApplicationManagers
{
	internal class MainApplicationManager : MonoBehaviour
	{
		private static bool _firstLaunch = true;

		private static MainApplicationManager _instance;

		public static void Init()
		{
			if (_firstLaunch)
			{
				_firstLaunch = false;
				_instance = SingletonFactory.CreateSingleton(_instance);
				ApplicationStart();
			}
			else if (AssetBundleManager.Status == AssetBundleStatus.Ready)
			{
				UIManager.SetMenu(MenuType.Main);
				GameProgressManager.OnMainMenu();
			}
			IN_GAME_MAIN_CAMERA.ApplyGraphicsSettings();
		}

		private static void ApplicationStart()
		{
			Language.init();
			Language.type = 0;
			DebugConsole.Init();
			ApplicationConfig.Init();
			AutoUpdateManager.Init();
			LevelInfo.Init();
			SettingsManager.Init();
			FullscreenHandler.Init();
			UIManager.Init();
			AssetBundleManager.Init();
			SnapshotManager.Init();
			CursorManager.Init();
			WeatherManager.Init();
			GameProgressManager.Init();
			MapManager.Init();
			MaterialCache.Init();
			if (ApplicationConfig.DevelopmentMode)
			{
				DebugTesting.Init();
				DebugTesting.RunTests();
			}
		}

		public static void FinishLoadAssets()
		{
			GameProgressManager.FinishLoadAssets();
			UIManager.FinishLoadAssets();
			CursorManager.FinishLoadAssets();
			WeatherManager.FinishLoadAssets();
		}
	}
}
