namespace Settings
{
	internal interface ICustomSkinSettings : ISetSettingsContainer
	{
		BoolSetting GetSkinsLocal();

		BoolSetting GetSkinsEnabled();

		IListSetting GetSkinSets();
	}
}
