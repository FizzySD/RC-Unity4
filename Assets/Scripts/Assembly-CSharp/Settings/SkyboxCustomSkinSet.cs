namespace Settings
{
	internal class SkyboxCustomSkinSet : BaseSetSetting
	{
		public SkyboxCustomSkinSet()
		{
		
		}

        public StringSetting Front = new StringSetting(string.Empty, 200);

		public StringSetting Back = new StringSetting(string.Empty, 200);

		public StringSetting Left = new StringSetting(string.Empty, 200);

		public StringSetting Right = new StringSetting(string.Empty, 200);

		public StringSetting Up = new StringSetting(string.Empty, 200);

		public StringSetting Down = new StringSetting(string.Empty, 200);
	}
}
