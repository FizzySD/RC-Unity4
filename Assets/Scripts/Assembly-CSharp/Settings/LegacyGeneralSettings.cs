namespace Settings
{
	internal class LegacyGeneralSettings : BaseSettingsContainer
	{
		public BoolSetting SpecMode = new BoolSetting(false);

		public BoolSetting LiveSpectate = new BoolSetting(true);
	}
}
