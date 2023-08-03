using Settings;
using UnityEngine;

namespace Utility
{
	internal class SettingsUtil
	{
		public static void SetSettingValue(BaseSetting setting, SettingType type, object value)
		{
			switch (type)
			{
			case SettingType.Bool:
				((BoolSetting)setting).Value = (bool)value;
				break;
			case SettingType.Color:
				((ColorSetting)setting).Value = (Color)value;
				break;
			case SettingType.Float:
				((FloatSetting)setting).Value = (float)value;
				break;
			case SettingType.Int:
				((IntSetting)setting).Value = (int)value;
				break;
			case SettingType.String:
				((StringSetting)setting).Value = (string)value;
				break;
			default:
				Debug.Log("Attempting to set invalid setting value.");
				break;
			}
		}

		public static object DeserializeValueFromJson(SettingType type, string json)
		{
			BaseSetting baseSetting = CreateBaseSetting(type);
			if (baseSetting == null)
			{
				return baseSetting;
			}
			baseSetting.DeserializeFromJsonString(json);
			return baseSetting;
		}

		public static BaseSetting CreateBaseSetting(SettingType type)
		{
			switch (type)
			{
			case SettingType.Bool:
				return new BoolSetting();
			case SettingType.Int:
				return new IntSetting();
			case SettingType.Float:
				return new FloatSetting();
			case SettingType.Color:
				return new ColorSetting();
			case SettingType.String:
				return new StringSetting();
			default:
				return null;
			}
		}
	}
}
