using System;
using ApplicationManagers;
using UnityEngine;

namespace Settings
{
	internal class GraphicsSettings : SaveableSettingsContainer
	{
		public IntSetting OverallQuality = new IntSetting(QualitySettings.GetQualityLevel());

		public IntSetting TextureQuality = new IntSetting(3);

		public BoolSetting VSync = new BoolSetting(false);

		public IntSetting FPSCap = new IntSetting(0, 0);

		public BoolSetting ExclusiveFullscreen = new BoolSetting(false);

		public BoolSetting ShowFPS = new BoolSetting(false);

		public BoolSetting MipmapEnabled = new BoolSetting(true);

		public BoolSetting RecordingClipAfterKill = new BoolSetting(true);

		public BoolSetting WeaponTrailEnabled = new BoolSetting(true);

		public BoolSetting WindEffectEnabled = new BoolSetting(false);

		public BoolSetting InterpolationEnabled = new BoolSetting(true);

		public IntSetting RenderDistance = new IntSetting(1500, 10, 1000000);

		public IntSetting WeatherEffects = new IntSetting(3);

		public BoolSetting AnimatedIntro = new BoolSetting(true);

		public BoolSetting BlurEnabled = new BoolSetting(false);

		public IntSetting AntiAliasing = new IntSetting(0);

		protected override string FileName
		{
			get
			{
				return "Graphics.json";
			}
		}

		public override void Save()
		{
			base.Save();
			FullscreenHandler.SetMainData(ExclusiveFullscreen.Value);
		}

		public override void Load()
		{
			base.Load();
			FullscreenHandler.SetMainData(ExclusiveFullscreen.Value);
		}

		public override void Apply()
		{
			QualitySettings.SetQualityLevel(OverallQuality.Value, true);
			QualitySettings.vSyncCount = Convert.ToInt32(VSync.Value);
			Application.targetFrameRate = ((FPSCap.Value > 0) ? FPSCap.Value : (-1));
			QualitySettings.masterTextureLimit = 3 - TextureQuality.Value;
			QualitySettings.antiAliasing = ((AntiAliasing.Value != 0) ? ((int)Mathf.Pow(2f, AntiAliasing.Value)) : 0);
			ApplyShadows();
			IN_GAME_MAIN_CAMERA.ApplyGraphicsSettings();
		}

		private void ApplyShadows()
		{
		}
	}
}
