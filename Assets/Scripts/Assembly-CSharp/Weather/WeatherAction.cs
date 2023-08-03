namespace Weather
{
	public enum WeatherAction
	{
		BeginSchedule = 0,
		EndSchedule = 1,
		RepeatNext = 2,
		BeginRepeat = 3,
		EndRepeat = 4,
		SetDefaultAll = 5,
		SetDefault = 6,
		SetValue = 7,
		SetTargetDefaultAll = 8,
		SetTargetDefault = 9,
		SetTargetValue = 10,
		SetTargetTimeAll = 11,
		SetTargetTime = 12,
		Wait = 13,
		Goto = 14,
		Label = 15,
		Return = 16,
		LoadSkybox = 17
	}
}
