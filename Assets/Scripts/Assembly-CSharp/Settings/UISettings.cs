using UI;

namespace Settings
{
	internal class UISettings : SaveableSettingsContainer
	{
		public StringSetting UITheme = new StringSetting("Dark");

		public BoolSetting GameFeed = new BoolSetting(false);

		public FloatSetting UIMasterScale = new FloatSetting(1f, 0.75f, 1.5f);

		public FloatSetting CrosshairScale = new FloatSetting(1f, 0f, 3f);

		public BoolSetting ShowCrosshairArrows = new BoolSetting(true);

		public BoolSetting ShowCrosshairDistance = new BoolSetting(true);

		public IntSetting CrosshairStyle = new IntSetting(0);

		public IntSetting Speedometer = new IntSetting(0);

		public BoolSetting ShowInterpolation = new BoolSetting(false);

		public BoolSetting ShowEmotes = new BoolSetting(true);

		public BoolSetting HideNames = new BoolSetting(false);

		public BoolSetting DisableNameColors = new BoolSetting(false);

		public IntSetting ChatLines = new IntSetting(15);

		public IntSetting ChatWidth = new IntSetting(150);

		public IntSetting ChatHeight = new IntSetting(100);

		protected override string FileName
		{
			get
			{
				return "UI.json";
			}
		}

		public override void Apply()
		{
			base.Apply();
			if (UIManager.CurrentMenu != null)
			{
				UIManager.CurrentMenu.ApplyScale();
			}
		}
	}
}
