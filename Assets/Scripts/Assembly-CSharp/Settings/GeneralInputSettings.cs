namespace Settings
{
	internal class GeneralInputSettings : SaveableSettingsContainer
	{
		public KeybindSetting Forward = new KeybindSetting(new string[2] { "W", "None" });

		public KeybindSetting Back = new KeybindSetting(new string[2] { "S", "None" });

		public KeybindSetting Left = new KeybindSetting(new string[2] { "A", "None" });

		public KeybindSetting Right = new KeybindSetting(new string[2] { "D", "None" });

		public KeybindSetting Pause = new KeybindSetting(new string[2] { "P", "None" });

		public KeybindSetting ChangeCharacter = new KeybindSetting(new string[2] { "T", "None" });

		public KeybindSetting RestartGame = new KeybindSetting(new string[2] { "F5", "None" });

		public KeybindSetting Chat = new KeybindSetting(new string[2] { "Return", "None" });

		public KeybindSetting ToggleFullscreen = new KeybindSetting(new string[2] { "Backspace", "None" });

		public KeybindSetting ChangeCamera = new KeybindSetting(new string[2] { "C", "None" });

		public KeybindSetting HideUI = new KeybindSetting(new string[2] { "X", "None" });

		public KeybindSetting MinimapReset = new KeybindSetting(new string[2] { "K", "None" });

		public KeybindSetting MinimapToggle = new KeybindSetting(new string[2] { "LeftControl+M", "None" });

		public KeybindSetting MinimapMaximize = new KeybindSetting(new string[2] { "M", "None" });

		public KeybindSetting JoinGame = new KeybindSetting(new string[2] { "1", "None" });

		public KeybindSetting SpectateToggleLive = new KeybindSetting(new string[2] { "Y", "None" });

		public KeybindSetting SpectateToggleFreeCamera = new KeybindSetting(new string[2] { "Mouse1", "None" });

		public KeybindSetting SpectatePreviousPlayer = new KeybindSetting(new string[2] { "1", "None" });

		public KeybindSetting SpectateNextPlayer = new KeybindSetting(new string[2] { "2", "None" });

		protected override string FileName
		{
			get
			{
				return "GeneralInput.json";
			}
		}
	}
}
