using System.Collections.Generic;
using Settings;
using UnityEngine;
using Utility;

namespace Weather
{
	internal class WeatherScheduleRunner
	{
		private const int ScheduleMaxRecursion = 200;

		private int _currentScheduleLine;

		private LinkedList<int> _callerStack = new LinkedList<int>();

		private Dictionary<string, int> _scheduleLabels = new Dictionary<string, int>();

		private Dictionary<int, int> _repeatStartLines = new Dictionary<int, int>();

		private Dictionary<int, int> _repeatCurrentCounts = new Dictionary<int, int>();

		private WeatherManager _manager;

		public WeatherSchedule Schedule = new WeatherSchedule();

		public WeatherScheduleRunner(WeatherManager manager)
		{
			_manager = manager;
		}

		public void ProcessSchedule()
		{
			for (int i = 0; i < Schedule.Events.Count - 1; i++)
			{
				if (Schedule.Events[i].Action == WeatherAction.RepeatNext)
				{
					Schedule.Events[i].Action = WeatherAction.BeginRepeat;
					WeatherEvent item = new WeatherEvent
					{
						Action = WeatherAction.EndRepeat
					};
					Schedule.Events.Insert(i + 2, item);
				}
			}
			int num = -1;
			for (int j = 0; j < Schedule.Events.Count; j++)
			{
				WeatherEvent weatherEvent = Schedule.Events[j];
				if (weatherEvent.Action == WeatherAction.Label)
				{
					string key = (string)weatherEvent.GetValue();
					if (!_scheduleLabels.ContainsKey(key))
					{
						_scheduleLabels.Add(key, j);
					}
				}
				else if (weatherEvent.Action == WeatherAction.BeginRepeat)
				{
					num = j;
					_repeatCurrentCounts.Add(j, 0);
				}
				else if (weatherEvent.Action == WeatherAction.EndRepeat && num >= 0)
				{
					_repeatStartLines.Add(j, num);
					num = -1;
				}
			}
		}

		public void ConsumeSchedule()
		{
			int num = 0;
			bool flag = false;
			while (!flag)
			{
				num++;
				if (num > 200)
				{
					Debug.Log("Weather schedule reached max usage (did you forget to use waits?)");
					Schedule.Events.Clear();
					break;
				}
				if (Schedule.Events.Count == 0 || _currentScheduleLine < 0 || _currentScheduleLine >= Schedule.Events.Count)
				{
					break;
				}
				WeatherEvent weatherEvent = Schedule.Events[_currentScheduleLine];
				switch (weatherEvent.Action)
				{
				case WeatherAction.SetDefaultAll:
				{
					bool value = _manager._currentWeather.ScheduleLoop.Value;
					_manager._currentWeather.SetDefault();
					_manager._currentWeather.UseSchedule.Value = true;
					_manager._currentWeather.ScheduleLoop.Value = value;
					_manager._needSync = true;
					break;
				}
				case WeatherAction.SetDefault:
					((BaseSetting)_manager._currentWeather.Settings[weatherEvent.Effect.ToString()]).SetDefault();
					_manager._needSync = true;
					break;
				case WeatherAction.SetValue:
				{
					BaseSetting setting = (BaseSetting)_manager._currentWeather.Settings[weatherEvent.Effect.ToString()];
					SettingsUtil.SetSettingValue(setting, weatherEvent.GetSettingType(), weatherEvent.GetValue());
					_manager._needSync = true;
					break;
				}
				case WeatherAction.SetTargetDefaultAll:
					_manager._targetWeather.SetDefault();
					_manager._needSync = true;
					break;
				case WeatherAction.SetTargetDefault:
					((BaseSetting)_manager._targetWeather.Settings[weatherEvent.Effect.ToString()]).SetDefault();
					_manager._needSync = true;
					break;
				case WeatherAction.SetTargetValue:
				{
					BaseSetting setting2 = (BaseSetting)_manager._targetWeather.Settings[weatherEvent.Effect.ToString()];
					SettingsUtil.SetSettingValue(setting2, weatherEvent.GetSettingType(), weatherEvent.GetValue());
					_manager._needSync = true;
					break;
				}
				case WeatherAction.SetTargetTime:
				{
					if (!_manager._targetWeatherStartTimes.ContainsKey((int)weatherEvent.Effect))
					{
						_manager._targetWeatherStartTimes.Add((int)weatherEvent.Effect, 0f);
						_manager._targetWeatherEndTimes.Add((int)weatherEvent.Effect, 0f);
					}
					BaseSetting baseSetting = (BaseSetting)_manager._startWeather.Settings[weatherEvent.Effect.ToString()];
					BaseSetting other = (BaseSetting)_manager._currentWeather.Settings[weatherEvent.Effect.ToString()];
					_manager._targetWeatherStartTimes[(int)weatherEvent.Effect] = _manager._currentTime;
					_manager._targetWeatherEndTimes[(int)weatherEvent.Effect] = _manager._currentTime + (float)weatherEvent.GetValue();
					baseSetting.Copy(other);
					_manager._needSync = true;
					break;
				}
				case WeatherAction.SetTargetTimeAll:
				{
					_manager._targetWeatherStartTimes.Clear();
					_manager._targetWeatherEndTimes.Clear();
					float value2 = _manager._currentTime + (float)weatherEvent.GetValue();
					foreach (WeatherEffect item in RCextensions.EnumToList<WeatherEffect>())
					{
						_manager._targetWeatherStartTimes.Add((int)item, _manager._currentTime);
						_manager._targetWeatherEndTimes.Add((int)item, value2);
					}
					_manager._needSync = true;
					break;
				}
				case WeatherAction.Wait:
					_manager._currentScheduleWait[this] = (float)weatherEvent.GetValue();
					flag = true;
					break;
				case WeatherAction.Goto:
				{
					string text = (string)weatherEvent.GetValue();
					if (text != "NextLine" && _scheduleLabels.ContainsKey(text))
					{
						_callerStack.AddLast(_currentScheduleLine);
						if (_callerStack.Count > 200)
						{
							_callerStack.RemoveFirst();
						}
						_currentScheduleLine = _scheduleLabels[text];
					}
					break;
				}
				case WeatherAction.Return:
					if (_callerStack.Count > 0)
					{
						_currentScheduleLine = _callerStack.Last.Value;
						_callerStack.RemoveLast();
					}
					break;
				case WeatherAction.BeginRepeat:
					_repeatCurrentCounts[_currentScheduleLine] = (int)weatherEvent.GetValue();
					break;
				case WeatherAction.EndRepeat:
				{
					int num2 = _repeatStartLines[_currentScheduleLine];
					if (_repeatCurrentCounts.ContainsKey(num2) && _repeatCurrentCounts[num2] > 0)
					{
						_currentScheduleLine = num2 + 1;
						_repeatCurrentCounts[num2]--;
					}
					break;
				}
				}
				_currentScheduleLine++;
				if (_currentScheduleLine >= Schedule.Events.Count && _manager._currentWeather.ScheduleLoop.Value)
				{
					_currentScheduleLine = 0;
				}
			}
		}
	}
}
