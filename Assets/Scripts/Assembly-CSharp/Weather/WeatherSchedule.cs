using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weather
{
	internal class WeatherSchedule
	{
		private static Dictionary<string, WeatherAction> NameToWeatherAction = RCextensions.EnumToDict<WeatherAction>();

		private static Dictionary<string, WeatherEffect> NameToWeatherEffect = RCextensions.EnumToDict<WeatherEffect>();

		private static Dictionary<string, WeatherValueSelectType> NameToWeatherValueSelectType = RCextensions.EnumToDict<WeatherValueSelectType>();

		public List<WeatherEvent> Events = new List<WeatherEvent>();

		public WeatherSchedule()
		{
		}

		public WeatherSchedule(string csv)
		{
			DeserializeFromCSV(csv);
		}

		public string SerializeToCSV()
		{
			List<string> list = new List<string>();
			foreach (WeatherEvent @event in Events)
			{
				List<string> list2 = new List<string>();
				list2.Add(@event.Action.ToString());
				if (@event.Effect != 0)
				{
					list2.Add(@event.Effect.ToString());
				}
				if (@event.ValueSelectType != 0)
				{
					if (@event.Action != WeatherAction.Label)
					{
						list2.Add(@event.ValueSelectType.ToString());
					}
					if (@event.ValueSelectType == WeatherValueSelectType.RandomFromList)
					{
						for (int i = 0; i < @event.Values.Count; i++)
						{
							list2.Add(SerializeRandomListValue(@event.GetValueType(), @event.Values[i], @event.Weights[i]));
						}
					}
					else
					{
						foreach (object value in @event.Values)
						{
							list2.Add(SerializeValue(@event.GetValueType(), value));
						}
					}
				}
				list.Add(string.Join(",", list2.ToArray()));
			}
			return string.Join(";\n", list.ToArray());
		}

		public string DeserializeFromCSV(string csv)
		{
			Events.Clear();
			string[] array = csv.Split(';');
			int num = 1;
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					string text = array[i].Trim();
					num += array[i].Split('\n').Length - 1;
					if (text != string.Empty && !text.StartsWith("//"))
					{
						Events.Add(DeserializeLine(text));
					}
				}
				catch (Exception)
				{
					return string.Format("Import failed at line {0}", num);
				}
			}
			return "";
		}

		private string SerializeValue(WeatherValueType type, object value)
		{
			string text = "";
			switch (type)
			{
			case WeatherValueType.String:
				text = (string)value;
				break;
			case WeatherValueType.Float:
				text = ((float)value).ToString();
				break;
			case WeatherValueType.Int:
				text = ((int)value).ToString();
				break;
			case WeatherValueType.Bool:
				text = Convert.ToInt32((bool)value).ToString();
				break;
			case WeatherValueType.Color:
				text = SerializeColor((Color)value);
				break;
			}
			text = text.Replace(",", string.Empty);
			return text.Replace(";", string.Empty);
		}

		private string SerializeRandomListValue(WeatherValueType type, object value, float weight)
		{
			return SerializeValue(type, value) + "-" + weight;
		}

		private string SerializeColor(Color color)
		{
			string[] array = new string[4]
			{
				color.r.ToString(),
				color.g.ToString(),
				color.b.ToString(),
				color.a.ToString()
			};
			if (color.a == 1f && color.r == color.g && color.r == color.b)
			{
				return array[0];
			}
			return string.Join("-", array);
		}

		private WeatherEvent DeserializeLine(string line)
		{
			WeatherEvent weatherEvent = new WeatherEvent();
			string[] array = line.Split(',');
			int num = 0;
			weatherEvent.Action = NameToWeatherAction[array[num++]];
			if (weatherEvent.SupportsWeatherEffects())
			{
				weatherEvent.Effect = NameToWeatherEffect[array[num++]];
			}
			if (weatherEvent.Action == WeatherAction.Label)
			{
				weatherEvent.ValueSelectType = WeatherValueSelectType.Constant;
			}
			else if (weatherEvent.SupportsWeatherValueSelectTypes())
			{
				weatherEvent.ValueSelectType = NameToWeatherValueSelectType[array[num++]];
			}
			if (weatherEvent.ValueSelectType == WeatherValueSelectType.RandomFromList)
			{
				for (int i = num; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('-');
					weatherEvent.Values.Add(DeserializeValue(weatherEvent.GetValueType(), array2[0]));
					if (array2.Length > 1)
					{
						weatherEvent.Weights.Add(float.Parse(array2[1]));
					}
					else
					{
						weatherEvent.Weights.Add(1f);
					}
				}
			}
			else
			{
				for (int j = num; j < array.Length; j++)
				{
					weatherEvent.Values.Add(DeserializeValue(weatherEvent.GetValueType(), array[j]));
				}
			}
			return weatherEvent;
		}

		private object DeserializeValue(WeatherValueType type, string item)
		{
			switch (type)
			{
			case WeatherValueType.String:
				return item;
			case WeatherValueType.Float:
				return float.Parse(item);
			case WeatherValueType.Int:
				return int.Parse(item);
			case WeatherValueType.Bool:
				return Convert.ToBoolean(int.Parse(item));
			case WeatherValueType.Color:
				return DeserializeColor(item);
			default:
				return null;
			}
		}

		private Color DeserializeColor(string item)
		{
			string[] array = item.Split('-');
			if (array.Length == 1)
			{
				float num = float.Parse(array[0]);
				return new Color(num, num, num, 1f);
			}
			return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		}
	}
}
