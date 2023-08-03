namespace Settings
{
	internal class TitanInputSettings : SaveableSettingsContainer
	{
		public KeybindSetting AttackPunch = new KeybindSetting(new string[2] { "Q", "None" });

		public KeybindSetting AttackSlam = new KeybindSetting(new string[2] { "E", "None" });

		public KeybindSetting AttackSlap = new KeybindSetting(new string[2] { "Mouse0", "None" });

		public KeybindSetting AttackGrabFront = new KeybindSetting(new string[2] { "1", "None" });

		public KeybindSetting AttackGrabBack = new KeybindSetting(new string[2] { "3", "None" });

		public KeybindSetting AttackGrabNape = new KeybindSetting(new string[2] { "Mouse1", "None" });

		public KeybindSetting AttackBite = new KeybindSetting(new string[2] { "2", "None" });

		public KeybindSetting CoverNape = new KeybindSetting(new string[2] { "Z", "None" });

		public KeybindSetting Jump = new KeybindSetting(new string[2] { "Space", "None" });

		public KeybindSetting Sit = new KeybindSetting(new string[2] { "X", "None" });

		public KeybindSetting Walk = new KeybindSetting(new string[2] { "LeftShift", "None" });

		protected override string FileName
		{
			get
			{
				return "TitanInput.json";
			}
		}
	}
}
