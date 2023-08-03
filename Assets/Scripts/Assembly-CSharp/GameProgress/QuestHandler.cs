using System;
using System.Collections.Generic;
using ApplicationManagers;
using Settings;
using UnityEngine;

namespace GameProgress
{
	internal class QuestHandler : BaseGameProgressHandler
	{
		protected QuestContainer _quest;

		protected Dictionary<string, List<QuestItem>> _activeQuests = new Dictionary<string, List<QuestItem>>();

		private const int DailyQuestCount = 3;

		private const int WeeklyQuestCount = 3;

		protected string[] TitanKillCategories = new string[1] { "KillTitan" };

		protected string[] HumanKillCategories = new string[1] { "KillHuman" };

		protected string[] DamageCategories = new string[2] { "DealDamage", "HitDamage" };

		protected string[] SpeedCategories = new string[1] { "ReachSpeed" };

		protected string[] InteractionCategories = new string[2] { "ShareGas", "CarryPlayer" };

		private static Dictionary<string, KillWeapon> NameToKillWeapon = RCextensions.EnumToDict<KillWeapon>();

		public QuestHandler(QuestContainer quest)
		{
			if (quest != null)
			{
				_quest = quest;
				ReloadQuests();
			}
		}

		public void ReloadQuests()
		{
			LoadQuests();
			CacheActiveQuests();
		}

		public static string GetTimeToQuestReset(bool daily)
		{
			TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
			if (daily)
			{
				int num = 24 - timeSpan.Hours;
				return string.Format("Resets in: {0} {1}", num, (num == 1) ? "hour" : "hours");
			}
			int num2 = (timeSpan.Days - 1) % 7;
			int num3 = 6 - num2;
			int num4 = 24 - timeSpan.Hours;
			return string.Format("Resets in: {0} {1}, {2} {3}", num3, (num3 == 1) ? "day" : "days", num4, (num4 == 1) ? "hour" : "hours");
		}

		private void LoadQuests()
		{
			int days = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Days;
			int num = (days - 1) / 7;
			QuestContainer questContainer = new QuestContainer();
			questContainer.DeserializeFromJsonString(((TextAsset)AssetBundleManager.MainAssetBundle.Load("QuestList")).text);
			ListSetting<QuestItem> listSetting = new ListSetting<QuestItem>();
			foreach (QuestItem item in _quest.DailyQuestItems.Value)
			{
				if (item.DayCreated.Value == days)
				{
					listSetting.Value.Add(item);
				}
			}
			listSetting.Value.AddRange(CreateQuests(questContainer, days, true, 3 - listSetting.Value.Count));
			ListSetting<QuestItem> listSetting2 = new ListSetting<QuestItem>();
			foreach (QuestItem item2 in _quest.WeeklyQuestItems.Value)
			{
				int num2 = (item2.DayCreated.Value - 1) / 7;
				if (num2 == num)
				{
					listSetting2.Value.Add(item2);
				}
			}
			listSetting2.Value.AddRange(CreateQuests(questContainer, days, false, 3 - listSetting2.Value.Count));
			_quest.DailyQuestItems.Copy(listSetting);
			_quest.WeeklyQuestItems.Copy(listSetting2);
		}

		private List<QuestItem> CreateQuests(QuestContainer defaultQuest, int currentDay, bool daily, int count)
		{
			List<QuestItem> list = (daily ? defaultQuest.DailyQuestItems.Value : defaultQuest.WeeklyQuestItems.Value);
			List<QuestItem> list2 = new List<QuestItem>();
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					if (!hashSet.Contains(list[index].GetQuestName()))
					{
						QuestItem questItem = new QuestItem();
						questItem.Copy(list[index]);
						questItem.DayCreated.Value = currentDay;
						list2.Add(questItem);
						hashSet.Add(questItem.GetQuestName());
						break;
					}
				}
			}
			return list2;
		}

		private void CacheActiveQuests()
		{
			_activeQuests.Clear();
			foreach (QuestItem item in _quest.DailyQuestItems.Value)
			{
				if (item.Progress.Value < item.Amount.Value)
				{
					AddActiveQuest(item);
				}
			}
			foreach (QuestItem item2 in _quest.WeeklyQuestItems.Value)
			{
				if (item2.Progress.Value < item2.Amount.Value)
				{
					AddActiveQuest(item2);
				}
			}
		}

		protected void AddActiveQuest(QuestItem item)
		{
			string value = item.Category.Value;
			if (!_activeQuests.ContainsKey(value))
			{
				_activeQuests.Add(value, new List<QuestItem>());
			}
			_activeQuests[value].Add(item);
		}

		protected virtual bool CheckKillConditions(List<StringSetting> conditions, KillWeapon weapon)
		{
			foreach (StringSetting condition in conditions)
			{
				string[] array = condition.Value.Split(':');
				string text = array[0];
				string key = array[1];
				if (text == "Weapon" && NameToKillWeapon[key] != weapon)
				{
					return false;
				}
			}
			return true;
		}

		protected virtual bool CheckDamageConditions(List<StringSetting> conditions, KillWeapon weapon, int damage)
		{
			foreach (StringSetting condition in conditions)
			{
				string[] array = condition.Value.Split(':');
				string text = array[0];
				string text2 = array[1];
				if (text == "Weapon" && NameToKillWeapon[text2] != weapon)
				{
					return false;
				}
				if (text == "Damage" && damage < int.Parse(text2))
				{
					return false;
				}
			}
			return true;
		}

		protected virtual bool CheckSpeedConditions(List<StringSetting> conditions, GameObject character, float speed)
		{
			foreach (StringSetting condition in conditions)
			{
				string[] array = condition.Value.Split(':');
				string text = array[0];
				string s = array[1];
				if (text == "Speed" && speed < (float)int.Parse(s))
				{
					return false;
				}
			}
			return true;
		}

		public override void RegisterTitanKill(GameObject character, TITAN victim, KillWeapon weapon)
		{
			string[] titanKillCategories = TitanKillCategories;
			foreach (string text in titanKillCategories)
			{
				if (!_activeQuests.ContainsKey(text))
				{
					continue;
				}
				foreach (QuestItem item in _activeQuests[text])
				{
					if (CheckKillConditions(item.Conditions.Value, weapon) && text == "KillTitan")
					{
						item.AddProgress();
					}
				}
			}
		}

		public override void RegisterHumanKill(GameObject character, HERO victim, KillWeapon weapon)
		{
			string[] humanKillCategories = HumanKillCategories;
			foreach (string text in humanKillCategories)
			{
				if (!_activeQuests.ContainsKey(text))
				{
					continue;
				}
				foreach (QuestItem item in _activeQuests[text])
				{
					if (CheckKillConditions(item.Conditions.Value, weapon) && text == "KillHuman")
					{
						item.AddProgress();
					}
				}
			}
		}

		public override void RegisterDamage(GameObject character, GameObject victim, KillWeapon weapon, int damage)
		{
			string[] damageCategories = DamageCategories;
			foreach (string text in damageCategories)
			{
				if (!_activeQuests.ContainsKey(text))
				{
					continue;
				}
				foreach (QuestItem item in _activeQuests[text])
				{
					if (CheckDamageConditions(item.Conditions.Value, weapon, damage))
					{
						if (text == "HitDamage")
						{
							item.AddProgress();
						}
						else if (text == "DealDamage")
						{
							item.AddProgress(damage);
						}
					}
				}
			}
		}

		public override void RegisterSpeed(GameObject character, float speed)
		{
			string[] speedCategories = SpeedCategories;
			foreach (string text in speedCategories)
			{
				if (!_activeQuests.ContainsKey(text))
				{
					continue;
				}
				foreach (QuestItem item in _activeQuests[text])
				{
					if (CheckSpeedConditions(item.Conditions.Value, character, speed) && text == "ReachSpeed")
					{
						item.AddProgress();
					}
				}
			}
		}

		public override void RegisterInteraction(GameObject character, GameObject interact, InteractionType interactionType)
		{
			string[] interactionCategories = InteractionCategories;
			foreach (string text in interactionCategories)
			{
				if (!_activeQuests.ContainsKey(text))
				{
					continue;
				}
				foreach (QuestItem item in _activeQuests[text])
				{
					if (text == "ShareGas")
					{
						item.AddProgress();
					}
					else if (text == "CarryHuman")
					{
						item.AddProgress();
					}
				}
			}
		}
	}
}
