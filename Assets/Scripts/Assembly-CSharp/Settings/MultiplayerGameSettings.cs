namespace Settings
{
	internal class MultiplayerGameSettings : BaseSettingsContainer
	{
		public StringSetting Map = new StringSetting("The City");

		public StringSetting Name = new StringSetting("FoodForTitan", 100);

		public StringSetting Password = new StringSetting(string.Empty, 100);

		public IntSetting MaxPlayers = new IntSetting(10, 0, 255);

		public IntSetting MaxTime = new IntSetting(120, 1, 100000);

		public IntSetting Difficulty = new IntSetting(0);
	}
}
