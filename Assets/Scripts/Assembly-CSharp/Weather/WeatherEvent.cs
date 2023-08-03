using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Weather
{
	internal class WeatherEvent
	{
		private static string[] AllWeatherEffects = RCextensions.EnumToStringArrayExceptNone<WeatherEffect>();

		private static string[] AllWeatherValueSelectTypes = RCextensions.EnumToStringArrayExceptNone<WeatherValueSelectType>();

		public WeatherAction Action;

		public WeatherEffect Effect;

		public WeatherValueSelectType ValueSelectType;

		public List<object> Values = new List<object>();

		public List<float> Weights = new List<float>();

		public object GetValue()
		{
			WeatherValueType valueType = GetValueType();
			switch (ValueSelectType)
			{
			case WeatherValueSelectType.Constant:
				return Values[0];
			case WeatherValueSelectType.RandomBetween:
				switch (valueType)
				{
				case WeatherValueType.Float:
					return Random.Range((float)Values[0], (float)Values[1]);
				case WeatherValueType.Int:
					return Random.Range((int)Values[0], (int)Values[1] + 1);
				case WeatherValueType.Color:
				{
					Color color = (Color)Values[0];
					Color color2 = (Color)Values[1];
					if (color.IsGray() && color2.IsGray())
					{
						float num = Random.Range(color.r, color2.r);
						return new Color(num, num, num);
					}
					float r = Random.Range(Mathf.Min(color.r, color2.r), Mathf.Max(color.r, color2.r));
					float g = Random.Range(Mathf.Min(color.g, color2.g), Mathf.Max(color.g, color2.g));
					float b = Random.Range(Mathf.Min(color.b, color2.b), Mathf.Max(color.b, color2.b));
					float a = Random.Range(Mathf.Min(color.a, color2.a), Mathf.Max(color.a, color2.a));
					return new Color(r, g, b, a);
				}
				}
				break;
			case WeatherValueSelectType.RandomFromList:
				return GetRandomFromList();
			}
			return null;
		}

		private object GetRandomFromList()
		{
			float num = 0f;
			foreach (float weight in Weights)
			{
				num += weight;
			}
			float num2 = Random.Range(0f, num);
			float num3 = 0f;
			for (int i = 0; i < Values.Count; i++)
			{
				if (num2 >= num3 && num2 < num3 + Weights[i])
				{
					return Values[i];
				}
				num3 += Weights[i];
			}
			return Values[0];
		}

		public WeatherValueType GetValueType()
		{
			switch (Action)
			{
			case WeatherAction.BeginSchedule:
			case WeatherAction.EndSchedule:
			case WeatherAction.EndRepeat:
			case WeatherAction.SetDefaultAll:
			case WeatherAction.SetDefault:
			case WeatherAction.SetTargetDefaultAll:
			case WeatherAction.SetTargetDefault:
			case WeatherAction.Return:
				return WeatherValueType.None;
			case WeatherAction.SetTargetTimeAll:
			case WeatherAction.SetTargetTime:
			case WeatherAction.Wait:
				return WeatherValueType.Float;
			case WeatherAction.Goto:
			case WeatherAction.Label:
			case WeatherAction.LoadSkybox:
				return WeatherValueType.String;
			case WeatherAction.RepeatNext:
			case WeatherAction.BeginRepeat:
				return WeatherValueType.Int;
			default:
				switch (Effect)
				{
				case WeatherEffect.Daylight:
				case WeatherEffect.AmbientLight:
				case WeatherEffect.SkyboxColor:
				case WeatherEffect.Flashlight:
				case WeatherEffect.FogColor:
					return WeatherValueType.Color;
				case WeatherEffect.FogDensity:
				case WeatherEffect.Rain:
				case WeatherEffect.Thunder:
				case WeatherEffect.Snow:
				case WeatherEffect.Wind:
					return WeatherValueType.Float;
				case WeatherEffect.Skybox:
					return WeatherValueType.String;
				default:
					return WeatherValueType.None;
				}
			}
		}

		public SettingType GetSettingType()
		{
			switch (GetValueType())
			{
			case WeatherValueType.Bool:
				return SettingType.Bool;
			case WeatherValueType.Color:
				return SettingType.Color;
			case WeatherValueType.Float:
				return SettingType.Float;
			case WeatherValueType.Int:
				return SettingType.Int;
			case WeatherValueType.String:
				return SettingType.String;
			default:
				return SettingType.None;
			}
		}

		public string[] SupportedWeatherEffects()
		{
			switch (Action)
			{
			case WeatherAction.SetDefault:
			case WeatherAction.SetValue:
			case WeatherAction.SetTargetDefault:
			case WeatherAction.SetTargetValue:
			case WeatherAction.SetTargetTime:
				return AllWeatherEffects;
			default:
				return new string[0];
			}
		}

		public bool SupportsWeatherEffects()
		{
			return SupportedWeatherEffects().Length != 0;
		}

		public string[] SupportedWeatherValueSelectTypes()
		{
			switch (GetValueType())
			{
			case WeatherValueType.Float:
			case WeatherValueType.Int:
			case WeatherValueType.Color:
				return AllWeatherValueSelectTypes;
			case WeatherValueType.String:
			case WeatherValueType.Bool:
				if (Action != WeatherAction.LoadSkybox && Action != WeatherAction.Label)
				{
					return new string[2]
					{
						WeatherValueSelectType.Constant.ToString(),
						WeatherValueSelectType.RandomFromList.ToString()
					};
				}
				return new string[1] { WeatherValueSelectType.Constant.ToString() };
			default:
				return new string[0];
			}
		}

		public bool SupportsWeatherValueSelectTypes()
		{
			return SupportedWeatherValueSelectTypes().Length != 0;
		}
	}
}
