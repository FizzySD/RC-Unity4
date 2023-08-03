using GameProgress;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class EditProfileStatsPanel : BasePanel
	{
		protected override float Width
		{
			get
			{
				return 720f;
			}
		}

		protected override float Height
		{
			get
			{
				return 520f;
			}
		}

		protected override bool DoublePanel
		{
			get
			{
				return true;
			}
		}

		protected override bool DoublePanelDivider
		{
			get
			{
				return true;
			}
		}

		protected override bool ScrollBar
		{
			get
			{
				return true;
			}
		}

		protected override float VerticalSpacing
		{
			get
			{
				return 10f;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			GameStatContainer gameStat = GameProgressManager.GameProgress.GameStat;
			AchievmentCount achievmentCount = GameProgressManager.GameProgress.Achievment.GetAchievmentCount();
			ElementStyle style = new ElementStyle(24, 100f, ThemePanel);
			CreateTitleLabel(DoublePanelLeft, style, "General");
			CreateStatLabel(DoublePanelLeft, style, "Level", gameStat.Level.Value.ToString());
			CreateStatLabel(DoublePanelLeft, style, "Exp", gameStat.Exp.Value + "/" + GameProgressManager.GetExpToNext());
			CreateStatLabel(DoublePanelLeft, style, "Hours played", (gameStat.PlayTime.Value / 3600f).ToString("0.00"));
			CreateStatLabel(DoublePanelLeft, style, "Highest speed", ((int)gameStat.HighestSpeed.Value).ToString());
			CreateHorizontalDivider(DoublePanelLeft);
			CreateTitleLabel(DoublePanelLeft, style, "Achievments");
			CreateStatLabel(DoublePanelLeft, style, "Bronze", achievmentCount.FinishedBronze + "/" + achievmentCount.TotalBronze);
			CreateStatLabel(DoublePanelLeft, style, "Silver", achievmentCount.FinishedSilver + "/" + achievmentCount.TotalSilver);
			CreateStatLabel(DoublePanelLeft, style, "Gold", achievmentCount.FinishedGold + "/" + achievmentCount.TotalGold);
			CreateHorizontalDivider(DoublePanelLeft);
			CreateTitleLabel(DoublePanelLeft, style, "Damage");
			CreateStatLabel(DoublePanelLeft, style, "Highest overall", gameStat.DamageHighestOverall.Value.ToString());
			CreateStatLabel(DoublePanelLeft, style, "Highest blade", gameStat.DamageHighestBlade.Value.ToString());
			CreateStatLabel(DoublePanelLeft, style, "Highest gun", gameStat.DamageHighestGun.Value.ToString());
			CreateStatLabel(DoublePanelLeft, style, "Total overall", gameStat.DamageTotalOverall.Value.ToString());
			CreateStatLabel(DoublePanelLeft, style, "Total blade", gameStat.DamageTotalBlade.Value.ToString());
			CreateStatLabel(DoublePanelLeft, style, "Total gun", gameStat.DamageTotalGun.Value.ToString());
			CreateTitleLabel(DoublePanelRight, style, "Titans Killed");
			CreateStatLabel(DoublePanelRight, style, "Total", gameStat.TitansKilledTotal.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Blade", gameStat.TitansKilledBlade.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Gun", gameStat.TitansKilledGun.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Thunder spear", gameStat.TitansKilledThunderSpear.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Other", gameStat.TitansKilledOther.Value.ToString());
			CreateHorizontalDivider(DoublePanelRight);
			CreateTitleLabel(DoublePanelRight, style, "Humans Killed");
			CreateStatLabel(DoublePanelRight, style, "Total", gameStat.HumansKilledTotal.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Blade", gameStat.HumansKilledBlade.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Gun", gameStat.HumansKilledGun.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Thunder spear", gameStat.HumansKilledThunderSpear.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Titan", gameStat.HumansKilledTitan.Value.ToString());
			CreateStatLabel(DoublePanelRight, style, "Other", gameStat.TitansKilledOther.Value.ToString());
		}

		protected void CreateStatLabel(Transform panel, ElementStyle style, string title, string value)
		{
			ElementFactory.CreateDefaultLabel(panel, style, title + ": " + value, FontStyle.Normal, TextAnchor.MiddleLeft).GetComponent<Text>();
		}

		protected void CreateTitleLabel(Transform panel, ElementStyle style, string title)
		{
			ElementFactory.CreateDefaultLabel(panel, style, title, FontStyle.Bold, TextAnchor.MiddleLeft).GetComponent<Text>();
		}
	}
}
