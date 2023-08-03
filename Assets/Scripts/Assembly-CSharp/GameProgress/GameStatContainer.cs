using Settings;

namespace GameProgress
{
	internal class GameStatContainer : BaseSettingsContainer
	{
		public IntSetting Level = new IntSetting(1);

		public IntSetting Exp = new IntSetting(0);

		public FloatSetting PlayTime = new FloatSetting(0f);

		public FloatSetting HighestSpeed = new FloatSetting(0f, 0f, 10000f);

		public IntSetting DamageHighestOverall = new IntSetting(0, 0, 10000);

		public IntSetting DamageHighestBlade = new IntSetting(0, 0, 10000);

		public IntSetting DamageHighestGun = new IntSetting(0, 0, 10000);

		public IntSetting DamageTotalOverall = new IntSetting(0, 0);

		public IntSetting DamageTotalBlade = new IntSetting(0, 0);

		public IntSetting DamageTotalGun = new IntSetting(0, 0);

		public IntSetting TitansKilledTotal = new IntSetting(0);

		public IntSetting TitansKilledBlade = new IntSetting(0);

		public IntSetting TitansKilledGun = new IntSetting(0);

		public IntSetting TitansKilledThunderSpear = new IntSetting(0);

		public IntSetting TitansKilledOther = new IntSetting(0);

		public IntSetting HumansKilledTotal = new IntSetting(0);

		public IntSetting HumansKilledBlade = new IntSetting(0);

		public IntSetting HumansKilledGun = new IntSetting(0);

		public IntSetting HumansKilledThunderSpear = new IntSetting(0);

		public IntSetting HumansKilledTitan = new IntSetting(0);

		public IntSetting HumansKilledOther = new IntSetting(0);
	}
}
