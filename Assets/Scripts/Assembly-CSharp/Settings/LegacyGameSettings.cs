namespace Settings
{
	internal class LegacyGameSettings : SaveableSettingsContainer
	{
		public IntSetting TitanSpawnCap = new IntSetting(20, 0, 100);

		public IntSetting GameType = new IntSetting(0, 0);

		public StringSetting LevelScript = new StringSetting(string.Empty);

		public StringSetting LogicScript = new StringSetting(string.Empty);

		public BoolSetting TitanNumberEnabled = new BoolSetting(false);

		public IntSetting TitanNumber = new IntSetting(10, 0, 100);

		public BoolSetting TitanSpawnEnabled = new BoolSetting(false);

		public FloatSetting TitanSpawnNormal = new FloatSetting(20f, 0f, 100f);

		public FloatSetting TitanSpawnAberrant = new FloatSetting(20f, 0f, 100f);

		public FloatSetting TitanSpawnJumper = new FloatSetting(20f, 0f, 100f);

		public FloatSetting TitanSpawnCrawler = new FloatSetting(20f, 0f, 100f);

		public FloatSetting TitanSpawnPunk = new FloatSetting(20f, 0f, 100f);

		public BoolSetting TitanSizeEnabled = new BoolSetting(false);

		public FloatSetting TitanSizeMin = new FloatSetting(1f, 0f, 100f);

		public FloatSetting TitanSizeMax = new FloatSetting(3f, 0f, 100f);

		public IntSetting TitanHealthMode = new IntSetting(0, 0);

		public IntSetting TitanHealthMin = new IntSetting(100, 0);

		public IntSetting TitanHealthMax = new IntSetting(200, 0);

		public BoolSetting TitanArmorEnabled = new BoolSetting(false);

		public BoolSetting TitanArmorCrawlerEnabled = new BoolSetting(false);

		public IntSetting TitanArmor = new IntSetting(1000, 0);

		public BoolSetting TitanExplodeEnabled = new BoolSetting(false);

		public IntSetting TitanExplodeRadius = new IntSetting(30, 0);

		public BoolSetting RockThrowEnabled = new BoolSetting(true);

		public BoolSetting PointModeEnabled = new BoolSetting(false);

		public IntSetting PointModeAmount = new IntSetting(25, 1);

		public BoolSetting BombModeEnabled = new BoolSetting(false);

		public BoolSetting BombModeCeiling = new BoolSetting(true);

		public BoolSetting BombModeInfiniteGas = new BoolSetting(true);

		public BoolSetting BombModeDisableTitans = new BoolSetting(true);

		public IntSetting TeamMode = new IntSetting(0, 0);

		public BoolSetting InfectionModeEnabled = new BoolSetting(false);

		public IntSetting InfectionModeAmount = new IntSetting(1, 1);

		public BoolSetting FriendlyMode = new BoolSetting(false);

		public IntSetting BladePVP = new IntSetting(0, 0);

		public BoolSetting AHSSAirReload = new BoolSetting(true);

		public BoolSetting CannonsFriendlyFire = new BoolSetting(false);

		public BoolSetting TitanPerWavesEnabled = new BoolSetting(false);

		public IntSetting TitanPerWaves = new IntSetting(10, 0, 100);

		public BoolSetting TitanMaxWavesEnabled = new BoolSetting(false);

		public IntSetting TitanMaxWaves = new IntSetting(20, 1);

		public BoolSetting PunksEveryFive = new BoolSetting(true);

		public BoolSetting GlobalMinimapDisable = new BoolSetting(false);

		public BoolSetting PreserveKDR = new BoolSetting(false);

		public BoolSetting RacingEndless = new BoolSetting(false);

		public FloatSetting RacingStartTime = new FloatSetting(20f, 1f);

		public BoolSetting EndlessRespawnEnabled = new BoolSetting(false);

		public IntSetting EndlessRespawnTime = new IntSetting(0, 5);

		public BoolSetting KickShifters = new BoolSetting(false);

		public BoolSetting AllowHorses = new BoolSetting(false);

		public StringSetting Motd = new StringSetting(string.Empty, 1000);

		public BoolSetting GlobalHideNames = new BoolSetting(false);

		protected override string FileName
		{
			get
			{
				return "LegacyGameSettings.json";
			}
		}
	}
}
