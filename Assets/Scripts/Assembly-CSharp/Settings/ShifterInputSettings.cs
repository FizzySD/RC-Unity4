namespace Settings
{
	internal class ShifterInputSettings : SaveableSettingsContainer
	{
		public KeybindSetting AttackDefault = new KeybindSetting(new string[2] { "Mouse0", "None" });

		public KeybindSetting AttackSpecial = new KeybindSetting(new string[2] { "Mouse1", "None" });

		public KeybindSetting CoverNape = new KeybindSetting(new string[2] { "Z", "None" });

		public KeybindSetting Jump = new KeybindSetting(new string[2] { "Space", "None" });

		public KeybindSetting Sit = new KeybindSetting(new string[2] { "X", "None" });

		public KeybindSetting Walk = new KeybindSetting(new string[2] { "LeftShift", "None" });

		public KeybindSetting Roar = new KeybindSetting(new string[2] { "N", "None" });

		protected override string FileName
		{
			get
			{
				return "ShifterInput.json";
			}
		}
	}
}
