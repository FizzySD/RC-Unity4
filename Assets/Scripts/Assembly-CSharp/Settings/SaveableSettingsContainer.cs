using System.IO;
using UnityEngine;

namespace Settings
{
	internal abstract class SaveableSettingsContainer : BaseSettingsContainer
	{
		protected virtual string FolderPath
		{
			get
			{
				return Application.dataPath + "/UserData/Settings";
			}
		}

		protected abstract string FileName { get; }

		protected virtual bool Encrypted
		{
			get
			{
				return false;
			}
		}

		protected override void Setup()
		{
			RegisterSettings();
			Load();
			Apply();
		}

		public virtual void Save()
		{
			Directory.CreateDirectory(FolderPath);
			string text = SerializeToJsonString();
			if (Encrypted)
			{
				text = new SimpleAES().Encrypt(text);
			}
			File.WriteAllText(GetFilePath(), text);
		}

		public virtual void Load()
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

		protected virtual void LoadLegacy()
		{
		}

		protected virtual string GetFilePath()
		{
			return FolderPath + "/" + FileName;
		}
	}
}
