namespace Settings
{
	internal class HumanInputSettings : SaveableSettingsContainer
	{
		public KeybindSetting AttackDefault = new KeybindSetting(new string[2] { "Mouse0", "None" });

		public KeybindSetting AttackSpecial = new KeybindSetting(new string[2] { "Mouse1", "None" });

		public KeybindSetting HookLeft = new KeybindSetting(new string[2] { "Q", "None" });

		public KeybindSetting HookRight = new KeybindSetting(new string[2] { "E", "None" });

		public KeybindSetting HookBoth = new KeybindSetting(new string[2] { "Space", "None" });

		public KeybindSetting Dash = new KeybindSetting(new string[2] { "LeftControl", "None" });

		public KeybindSetting ReelIn = new KeybindSetting(new string[2] { "WheelDown", "None" });

		public KeybindSetting ReelOut = new KeybindSetting(new string[2] { "WheelUp", "None" });

		public KeybindSetting Dodge = new KeybindSetting(new string[2] { "LeftControl", "None" });

		public KeybindSetting FocusTitan = new KeybindSetting(new string[2] { "None", "None" });

		public KeybindSetting Jump = new KeybindSetting(new string[2] { "LeftShift", "None" });

		public KeybindSetting Reload = new KeybindSetting(new string[2] { "R", "None" });

		public KeybindSetting HorseMount = new KeybindSetting(new string[2] { "LeftControl", "None" });

		public KeybindSetting HorseWalk = new KeybindSetting(new string[2] { "LeftShift", "None" });

		public KeybindSetting HorseJump = new KeybindSetting(new string[2] { "Q", "None" });

		public KeybindSetting Flare1 = new KeybindSetting(new string[2] { "Alpha1", "None" });

		public KeybindSetting Flare2 = new KeybindSetting(new string[2] { "Alpha2", "None" });

		public KeybindSetting Flare3 = new KeybindSetting(new string[2] { "Alpha3", "None" });

		public BoolSetting DashDoubleTap = new BoolSetting(true);

		public FloatSetting ReelOutScrollSmoothing = new FloatSetting(0.2f, 0f, 1f);

		protected override string FileName
		{
			get
			{
				return "HumanInput.json";
			}
		}
	}
}
