namespace Settings
{
	internal class RCEditorInputSettings : SaveableSettingsContainer
	{
		public KeybindSetting Up = new KeybindSetting(new string[2] { "Mouse1", "None" });

		public KeybindSetting Down = new KeybindSetting(new string[2] { "Mouse0", "None" });

		public KeybindSetting Slow = new KeybindSetting(new string[2] { "LeftShift", "None" });

		public KeybindSetting Fast = new KeybindSetting(new string[2] { "LeftControl", "None" });

		public KeybindSetting RotateRight = new KeybindSetting(new string[2] { "E", "None" });

		public KeybindSetting RotateLeft = new KeybindSetting(new string[2] { "Q", "None" });

		public KeybindSetting RotateCCW = new KeybindSetting(new string[2] { "Z", "None" });

		public KeybindSetting RotateCW = new KeybindSetting(new string[2] { "C", "None" });

		public KeybindSetting RotateBack = new KeybindSetting(new string[2] { "F", "None" });

		public KeybindSetting RotateForward = new KeybindSetting(new string[2] { "R", "None" });

		public KeybindSetting Place = new KeybindSetting(new string[2] { "Space", "None" });

		public KeybindSetting Delete = new KeybindSetting(new string[2] { "Backspace", "None" });

		public KeybindSetting Cursor = new KeybindSetting(new string[2] { "X", "None" });

		protected override string FileName
		{
			get
			{
				return "RCEditorInput.json";
			}
		}
	}
}
