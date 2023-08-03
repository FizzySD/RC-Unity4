using Settings;

namespace GameProgress
{
	internal class AchievmentContainer : BaseSettingsContainer
	{
		public ListSetting<AchievmentItem> AchievmentItems = new ListSetting<AchievmentItem>();

		public AchievmentCount GetAchievmentCount()
		{
			AchievmentCount achievmentCount = new AchievmentCount();
			foreach (AchievmentItem item in AchievmentItems.Value)
			{
				if (item.Tier.Value == "Bronze")
				{
					achievmentCount.TotalBronze++;
					if (item.Finished())
					{
						achievmentCount.FinishedBronze++;
					}
				}
				else if (item.Tier.Value == "Silver")
				{
					achievmentCount.TotalSilver++;
					if (item.Finished())
					{
						achievmentCount.FinishedSilver++;
					}
				}
				else if (item.Tier.Value == "Gold")
				{
					achievmentCount.TotalGold++;
					if (item.Finished())
					{
						achievmentCount.FinishedGold++;
					}
				}
			}
			achievmentCount.TotalAll = achievmentCount.TotalBronze + achievmentCount.TotalSilver + achievmentCount.TotalGold;
			achievmentCount.FinishedAll = achievmentCount.FinishedBronze + achievmentCount.FinishedSilver + achievmentCount.FinishedGold;
			return achievmentCount;
		}
	}
}
