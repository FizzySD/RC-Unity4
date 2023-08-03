using UnityEngine;

namespace Settings
{
	internal class ProfileSettings : SaveableSettingsContainer
	{
		public NameSetting Name = new NameSetting("GUEST" + Random.Range(0, 100000), 200, 40);

		public NameSetting Guild = new NameSetting(string.Empty, 200, 40);

		protected override string FileName
		{
			get
			{
				return "Profile.json";
			}
		}

		protected override void LoadLegacy()
		{
			Name.Value = SettingsManager.MultiplayerSettings.Name.Value;
			Guild.Value = SettingsManager.MultiplayerSettings.Guild.Value;
		}
	}
}
