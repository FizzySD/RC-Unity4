namespace Settings
{
	internal class CityCustomSkinSet : BaseSetSetting
	{
		public ListSetting<StringSetting> Houses = new ListSetting<StringSetting>(new StringSetting(string.Empty, 200), 8);

		public StringSetting Ground = new StringSetting(string.Empty, 200);

		public StringSetting Wall = new StringSetting(string.Empty, 200);

		public StringSetting Gate = new StringSetting(string.Empty, 200);

		protected override bool Validate()
		{
			return Houses.Value.Count == 8;
		}
	}
}
