using Settings;
using UnityEngine;

namespace GameProgress
{
	internal class GameProgressContainer : SaveableSettingsContainer
	{
		public AchievmentContainer Achievment = new AchievmentContainer();

		public QuestContainer Quest = new QuestContainer();

		public GameStatContainer GameStat = new GameStatContainer();

		protected override string FolderPath
		{
			get
			{
				return Application.dataPath + "/UserData/GameProgress";
			}
		}

		protected override string FileName
		{
			get
			{
				return "GameProgress";
			}
		}

		protected override bool Encrypted
		{
			get
			{
				return true;
			}
		}

		public override void Save()
		{
			Quest.CollectRewards();
			base.Save();
		}
	}
}
