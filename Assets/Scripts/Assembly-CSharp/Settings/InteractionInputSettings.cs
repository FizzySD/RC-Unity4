namespace Settings
{
	internal class InteractionInputSettings : SaveableSettingsContainer
	{
		public KeybindSetting Interact = new KeybindSetting(new string[2] { "G", "None" });

		public KeybindSetting CannonSlow = new KeybindSetting(new string[2] { "LeftShift", "None" });

		public KeybindSetting CannonFire = new KeybindSetting(new string[2] { "Q", "None" });

		public KeybindSetting EmoteMenu = new KeybindSetting(new string[2] { "N", "None" });

		public KeybindSetting MenuNext = new KeybindSetting(new string[2] { "Space", "None" });

		public KeybindSetting QuickSelect1 = new KeybindSetting(new string[2] { "Alpha1", "None" });

		public KeybindSetting QuickSelect2 = new KeybindSetting(new string[2] { "Alpha2", "None" });

		public KeybindSetting QuickSelect3 = new KeybindSetting(new string[2] { "Alpha3", "None" });

		public KeybindSetting QuickSelect4 = new KeybindSetting(new string[2] { "Alpha4", "None" });

		public KeybindSetting QuickSelect5 = new KeybindSetting(new string[2] { "Alpha5", "None" });

		public KeybindSetting QuickSelect6 = new KeybindSetting(new string[2] { "Alpha6", "None" });

		public KeybindSetting QuickSelect7 = new KeybindSetting(new string[2] { "Alpha7", "None" });

		public KeybindSetting QuickSelect8 = new KeybindSetting(new string[2] { "Alpha8", "None" });

		protected override string FileName
		{
			get
			{
				return "InteractionInput.json";
			}
		}
	}
}
