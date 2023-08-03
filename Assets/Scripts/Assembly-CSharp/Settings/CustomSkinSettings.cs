using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Settings
{
	internal class CustomSkinSettings : SaveableSettingsContainer
	{
		public HumanCustomSkinSettings Human = new HumanCustomSkinSettings();

		public BaseCustomSkinSettings<TitanCustomSkinSet> Titan = new BaseCustomSkinSettings<TitanCustomSkinSet>();

		public BaseCustomSkinSettings<ShifterCustomSkinSet> Shifter = new BaseCustomSkinSettings<ShifterCustomSkinSet>();

		public BaseCustomSkinSettings<SkyboxCustomSkinSet> Skybox = new BaseCustomSkinSettings<SkyboxCustomSkinSet>();

		public BaseCustomSkinSettings<ForestCustomSkinSet> Forest = new BaseCustomSkinSettings<ForestCustomSkinSet>();

		public BaseCustomSkinSettings<CityCustomSkinSet> City = new BaseCustomSkinSettings<CityCustomSkinSet>();

		public BaseCustomSkinSettings<CustomLevelCustomSkinSet> CustomLevel = new BaseCustomSkinSettings<CustomLevelCustomSkinSet>();

		protected override string FileName
		{
			get
			{
				return "CustomSkins.json";
			}
		}

		public override void Load()
		{
			string filePath = GetFilePath();
			if (File.Exists(filePath))
			{
				string text = File.ReadAllText(filePath);
				if (Encrypted)
				{
					text = new SimpleAES().Decrypt(text);
				}
				DeserializeFromJsonString(text);
				ICustomSkinSettings[] array = new ICustomSkinSettings[7] { Human, Titan, Shifter, Skybox, Forest, City, CustomLevel };
				foreach (ICustomSkinSettings customSkinSettings in array)
				{
					List<BaseSetting> items = customSkinSettings.GetSkinSets().GetItems();
					if (items.Count > 0)
					{
						customSkinSettings.GetSets().Clear();
						foreach (BaseSetSetting item in customSkinSettings.GetSkinSets().GetItems())
						{
							customSkinSettings.GetSets().AddItem(item);
						}
					}
					customSkinSettings.GetSkinSets().Clear();
				}
				return;
			}
			try
			{
				LoadLegacy();
			}
			catch
			{
				Debug.Log("Exception occurred while loading legacy settings.");
			}
		}
	}
}
