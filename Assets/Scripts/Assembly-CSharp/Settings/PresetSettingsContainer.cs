using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Settings
{
	internal abstract class PresetSettingsContainer : SaveableSettingsContainer
	{
		protected virtual string PresetFolderPath
		{
			get
			{
				return Application.dataPath + "/Resources/Presets";
			}
		}

		public override void Load()
		{
			string presetFilePath = GetPresetFilePath();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (File.Exists(presetFilePath))
			{
				DeserializeFromJsonString(File.ReadAllText(presetFilePath));
				foreach (string key in Settings.Keys)
				{
					dictionary.Add(key, ((BaseSetting)Settings[key]).SerializeToJsonString());
				}
			}
			SetDefault();
			base.Load();
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				BaseSetting baseSetting = (BaseSetting)Settings[item.Key];
				if (baseSetting is ISetSettingsContainer)
				{
					ISetSettingsContainer setSettingsContainer = (ISetSettingsContainer)baseSetting;
					setSettingsContainer.SetPresetsFromJsonString(item.Value);
				}
			}
		}

		protected virtual string GetPresetFilePath()
		{
			return PresetFolderPath + "/" + FileName;
		}
	}
}
