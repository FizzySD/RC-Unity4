using Settings;

namespace GameProgress
{
	internal class QuestContainer : BaseSettingsContainer
	{
		public ListSetting<QuestItem> DailyQuestItems = new ListSetting<QuestItem>();

		public ListSetting<QuestItem> WeeklyQuestItems = new ListSetting<QuestItem>();

		public void CollectRewards()
		{
			foreach (QuestItem item in DailyQuestItems.Value)
			{
				item.CollectReward();
			}
			foreach (QuestItem item2 in WeeklyQuestItems.Value)
			{
				item2.CollectReward();
			}
		}
	}
}
