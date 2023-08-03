namespace Settings
{
	internal class BaseCustomSkinSettings<T> : SetSettingsContainer<T>, ICustomSkinSettings, ISetSettingsContainer where T : BaseSetSetting, new()
	{
		public BoolSetting SkinsLocal = new BoolSetting(false);

		public BoolSetting SkinsEnabled = new BoolSetting(true);

		public ListSetting<T> SkinSets = new ListSetting<T>();

		public BoolSetting GetSkinsEnabled()
		{
			return SkinsEnabled;
		}

		public IListSetting GetSkinSets()
		{
			return SkinSets;
		}

		public BoolSetting GetSkinsLocal()
		{
			return SkinsLocal;
		}
	}
}
