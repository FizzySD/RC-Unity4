using GameProgress;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class QuestDailyPanel : QuestCategoryPanel
	{
		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			GameObject gameObject = ElementFactory.CreateDefaultLabel(SinglePanel, new ElementStyle(24, 120f, ThemePanel), QuestHandler.GetTimeToQuestReset(true), FontStyle.Normal, TextAnchor.MiddleLeft);
			gameObject.GetComponent<Text>().color = UIManager.GetThemeColor(ThemePanel, "QuestHeader", "ResetTextColor");
			CreateQuestItems(GameProgressManager.GameProgress.Quest.DailyQuestItems.Value);
		}
	}
}
