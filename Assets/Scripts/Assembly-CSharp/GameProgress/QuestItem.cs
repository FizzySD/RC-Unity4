using System;
using Settings;

namespace GameProgress
{
	internal class QuestItem : BaseSettingsContainer
	{
		public StringSetting Category = new StringSetting(string.Empty);

		public ListSetting<StringSetting> Conditions = new ListSetting<StringSetting>();

		public IntSetting Amount = new IntSetting(0);

		public StringSetting RewardType = new StringSetting(string.Empty);

		public StringSetting RewardValue = new StringSetting(string.Empty);

		public StringSetting Icon = new StringSetting(string.Empty);

		public IntSetting Progress = new IntSetting(0);

		public BoolSetting Daily = new BoolSetting(true);

		public IntSetting DayCreated = new IntSetting(0);

		public BoolSetting Collected = new BoolSetting(false);

		public string GetQuestName()
		{
			return Category.Value + GetConditionsHash() + Amount.Value;
		}

		public string GetConditionsHash()
		{
			string text = "";
			foreach (StringSetting item in Conditions.Value)
			{
				text += item.Value;
			}
			return text;
		}

		public bool Finished()
		{
			return Progress.Value >= Amount.Value;
		}

		public void AddProgress(int count = 1)
		{
			Progress.Value += count;
			Progress.Value = Math.Min(Progress.Value, Amount.Value);
		}

		public void CollectReward()
		{
			if (!Collected.Value && Progress.Value >= Amount.Value)
			{
				Collected.Value = true;
				if (RewardType.Value == "Exp")
				{
					GameProgressManager.AddExp(int.Parse(RewardValue.Value));
				}
			}
		}
	}
}
