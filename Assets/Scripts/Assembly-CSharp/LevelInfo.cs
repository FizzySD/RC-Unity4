using UnityEngine;

public class LevelInfo
{
	public string desc;

	public int enemyNumber;

	public bool hint;

	public bool horse;

	private static bool init;

	public bool lavaMode;

	public static LevelInfo[] levels;

	public string mapName;

	public Minimap.Preset minimapPreset;

	public string name;

	public bool noCrawler;

	public bool punk = true;

	public bool pvp;

	public RespawnMode respawnMode;

	public bool supply = true;

	public bool teamTitan;

	public GAMEMODE type;

	public static LevelInfo getInfo(string name)
	{
		Init();
		LevelInfo[] array = levels;
		foreach (LevelInfo levelInfo in array)
		{
			if (levelInfo.name == name)
			{
				return levelInfo;
			}
		}
		return null;
	}

	public static void Init()
	{
		if (!init)
		{
			init = true;
			levels = new LevelInfo[27]
			{
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo(),
				new LevelInfo()
			};
			levels[0].name = "The City";
			levels[0].mapName = "The City I";
			levels[0].desc = "kill all the titans with your friends.(No RESPAWN/SUPPLY/PLAY AS TITAN)";
			levels[0].enemyNumber = 10;
			levels[0].type = GAMEMODE.KILL_TITAN;
			levels[0].respawnMode = RespawnMode.NEVER;
			levels[0].supply = true;
			levels[0].teamTitan = true;
			levels[0].pvp = true;
			levels[1].name = "The City II";
			levels[1].mapName = "The City I";
			levels[1].desc = "Fight the titans with your friends.(RESPAWN AFTER 10 SECONDS/SUPPLY/TEAM TITAN)";
			levels[1].enemyNumber = 10;
			levels[1].type = GAMEMODE.KILL_TITAN;
			levels[1].respawnMode = RespawnMode.DEATHMATCH;
			levels[1].supply = true;
			levels[1].teamTitan = true;
			levels[1].pvp = true;
			levels[2].name = "Cage Fighting";
			levels[2].mapName = "Cage Fighting";
			levels[2].desc = "2 players in different cages. when you kill a titan,  one or more titan will spawn to your opponent's cage.";
			levels[2].enemyNumber = 1;
			levels[2].type = GAMEMODE.CAGE_FIGHT;
			levels[2].respawnMode = RespawnMode.NEVER;
			levels[3].name = "The Forest";
			levels[3].mapName = "The Forest";
			levels[3].desc = "The Forest Of Giant Trees.(No RESPAWN/SUPPLY/PLAY AS TITAN)";
			levels[3].enemyNumber = 5;
			levels[3].type = GAMEMODE.KILL_TITAN;
			levels[3].respawnMode = RespawnMode.NEVER;
			levels[3].supply = true;
			levels[3].teamTitan = true;
			levels[3].pvp = true;
			levels[4].name = "The Forest II";
			levels[4].mapName = "The Forest";
			levels[4].desc = "Survive for 20 waves.";
			levels[4].enemyNumber = 3;
			levels[4].type = GAMEMODE.SURVIVE_MODE;
			levels[4].respawnMode = RespawnMode.NEVER;
			levels[4].supply = true;
			levels[5].name = "The Forest III";
			levels[5].mapName = "The Forest";
			levels[5].desc = "Survive for 20 waves.player will respawn in every new wave";
			levels[5].enemyNumber = 3;
			levels[5].type = GAMEMODE.SURVIVE_MODE;
			levels[5].respawnMode = RespawnMode.NEWROUND;
			levels[5].supply = true;
			levels[6].name = "Annie";
			levels[6].mapName = "The Forest";
			levels[6].desc = "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:2500/100\nAbnormal:4000/200\nYou only have 1 life.Don't do this alone.";
			levels[6].enemyNumber = 15;
			levels[6].type = GAMEMODE.KILL_TITAN;
			levels[6].respawnMode = RespawnMode.NEVER;
			levels[6].punk = false;
			levels[6].pvp = true;
			levels[7].name = "Annie II";
			levels[7].mapName = "The Forest";
			levels[7].desc = "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:3000/200\nAbnormal:6000/1000\n(RESPAWN AFTER 10 SECONDS)";
			levels[7].enemyNumber = 15;
			levels[7].type = GAMEMODE.KILL_TITAN;
			levels[7].respawnMode = RespawnMode.DEATHMATCH;
			levels[7].punk = false;
			levels[7].pvp = true;
			levels[8].name = "Colossal Titan";
			levels[8].mapName = "Colossal Titan";
			levels[8].desc = "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:2000\nHard:3500\nAbnormal:5000\n";
			levels[8].enemyNumber = 2;
			levels[8].type = GAMEMODE.BOSS_FIGHT_CT;
			levels[8].respawnMode = RespawnMode.NEVER;
			levels[9].name = "Colossal Titan II";
			levels[9].mapName = "Colossal Titan";
			levels[9].desc = "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:5000\nHard:8000\nAbnormal:12000\n(RESPAWN AFTER 10 SECONDS)";
			levels[9].enemyNumber = 2;
			levels[9].type = GAMEMODE.BOSS_FIGHT_CT;
			levels[9].respawnMode = RespawnMode.DEATHMATCH;
			levels[10].name = "Trost";
			levels[10].mapName = "Colossal Titan";
			levels[10].desc = "Escort Titan Eren";
			levels[10].enemyNumber = 2;
			levels[10].type = GAMEMODE.TROST;
			levels[10].respawnMode = RespawnMode.NEVER;
			levels[10].punk = false;
			levels[11].name = "Trost II";
			levels[11].mapName = "Colossal Titan";
			levels[11].desc = "Escort Titan Eren(RESPAWN AFTER 10 SECONDS)";
			levels[11].enemyNumber = 2;
			levels[11].type = GAMEMODE.TROST;
			levels[11].respawnMode = RespawnMode.DEATHMATCH;
			levels[11].punk = false;
			levels[12].name = "[S]City";
			levels[12].mapName = "The City I";
			levels[12].desc = "Kill all 15 Titans";
			levels[12].enemyNumber = 15;
			levels[12].type = GAMEMODE.KILL_TITAN;
			levels[12].respawnMode = RespawnMode.NEVER;
			levels[12].supply = true;
			levels[13].name = "[S]Forest";
			levels[13].mapName = "The Forest";
			levels[13].desc = string.Empty;
			levels[13].enemyNumber = 15;
			levels[13].type = GAMEMODE.KILL_TITAN;
			levels[13].respawnMode = RespawnMode.NEVER;
			levels[13].supply = true;
			levels[14].name = "[S]Forest Survive(no crawler)";
			levels[14].mapName = "The Forest";
			levels[14].desc = string.Empty;
			levels[14].enemyNumber = 3;
			levels[14].type = GAMEMODE.SURVIVE_MODE;
			levels[14].respawnMode = RespawnMode.NEVER;
			levels[14].supply = true;
			levels[14].noCrawler = true;
			levels[14].punk = true;
			levels[15].name = "[S]Tutorial";
			levels[15].mapName = "tutorial";
			levels[15].desc = string.Empty;
			levels[15].enemyNumber = 1;
			levels[15].type = GAMEMODE.KILL_TITAN;
			levels[15].respawnMode = RespawnMode.NEVER;
			levels[15].supply = true;
			levels[15].hint = true;
			levels[15].punk = false;
			levels[16].name = "[S]Battle training";
			levels[16].mapName = "tutorial 1";
			levels[16].desc = string.Empty;
			levels[16].enemyNumber = 7;
			levels[16].type = GAMEMODE.KILL_TITAN;
			levels[16].respawnMode = RespawnMode.NEVER;
			levels[16].supply = true;
			levels[16].punk = false;
			levels[17].name = "The Forest IV  - LAVA";
			levels[17].mapName = "The Forest";
			levels[17].desc = "Survive for 20 waves.player will respawn in every new wave.\nNO CRAWLERS\n***YOU CAN'T TOUCH THE GROUND!***";
			levels[17].enemyNumber = 3;
			levels[17].type = GAMEMODE.SURVIVE_MODE;
			levels[17].respawnMode = RespawnMode.NEWROUND;
			levels[17].supply = true;
			levels[17].noCrawler = true;
			levels[17].lavaMode = true;
			levels[18].name = "[S]Racing - Akina";
			levels[18].mapName = "track - akina";
			levels[18].desc = string.Empty;
			levels[18].enemyNumber = 0;
			levels[18].type = GAMEMODE.RACING;
			levels[18].respawnMode = RespawnMode.NEVER;
			levels[18].supply = false;
			levels[19].name = "Racing - Akina";
			levels[19].mapName = "track - akina";
			levels[19].desc = string.Empty;
			levels[19].enemyNumber = 0;
			levels[19].type = GAMEMODE.RACING;
			levels[19].respawnMode = RespawnMode.NEVER;
			levels[19].supply = false;
			levels[19].pvp = true;
			levels[20].name = "Outside The Walls";
			levels[20].mapName = "OutSide";
			levels[20].desc = "Capture Checkpoint mode.";
			levels[20].enemyNumber = 0;
			levels[20].type = GAMEMODE.PVP_CAPTURE;
			levels[20].respawnMode = RespawnMode.DEATHMATCH;
			levels[20].supply = true;
			levels[20].horse = true;
			levels[20].teamTitan = true;
			levels[21].name = "The City III";
			levels[21].mapName = "The City I";
			levels[21].desc = "Capture Checkpoint mode.";
			levels[21].enemyNumber = 0;
			levels[21].type = GAMEMODE.PVP_CAPTURE;
			levels[21].respawnMode = RespawnMode.DEATHMATCH;
			levels[21].supply = true;
			levels[21].horse = false;
			levels[21].teamTitan = true;
			levels[22].name = "Cave Fight";
			levels[22].mapName = "CaveFight";
			levels[22].desc = "***Spoiler Alarm!***";
			levels[22].enemyNumber = -1;
			levels[22].type = GAMEMODE.PVP_AHSS;
			levels[22].respawnMode = RespawnMode.NEVER;
			levels[22].supply = true;
			levels[22].horse = false;
			levels[22].teamTitan = true;
			levels[22].pvp = true;
			levels[23].name = "House Fight";
			levels[23].mapName = "HouseFight";
			levels[23].desc = "***Spoiler Alarm!***";
			levels[23].enemyNumber = -1;
			levels[23].type = GAMEMODE.PVP_AHSS;
			levels[23].respawnMode = RespawnMode.NEVER;
			levels[23].supply = true;
			levels[23].horse = false;
			levels[23].teamTitan = true;
			levels[23].pvp = true;
			levels[24].name = "[S]Forest Survive(no crawler no punk)";
			levels[24].mapName = "The Forest";
			levels[24].desc = string.Empty;
			levels[24].enemyNumber = 3;
			levels[24].type = GAMEMODE.SURVIVE_MODE;
			levels[24].respawnMode = RespawnMode.NEVER;
			levels[24].supply = true;
			levels[24].noCrawler = true;
			levels[24].punk = false;
			levels[25].name = "Custom";
			levels[25].mapName = "The Forest";
			levels[25].desc = "Custom Map.";
			levels[25].enemyNumber = 1;
			levels[25].type = GAMEMODE.KILL_TITAN;
			levels[25].respawnMode = RespawnMode.NEVER;
			levels[25].supply = true;
			levels[25].teamTitan = true;
			levels[25].pvp = true;
			levels[25].punk = true;
			levels[26].name = "Custom (No PT)";
			levels[26].mapName = "The Forest";
			levels[26].desc = "Custom Map (No Player Titans).";
			levels[26].enemyNumber = 1;
			levels[26].type = GAMEMODE.KILL_TITAN;
			levels[26].respawnMode = RespawnMode.NEVER;
			levels[26].pvp = true;
			levels[26].punk = true;
			levels[26].supply = true;
			levels[26].teamTitan = false;
			levels[0].minimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 731.9738f);
			levels[8].minimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f);
			levels[9].minimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f);
			levels[18].minimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f);
			levels[19].minimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f);
			levels[20].minimapPreset = new Minimap.Preset(new Vector3(2549.4f, 0f, 3042.4f), 3697.16f);
			levels[21].minimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 734.9738f);
		}
	}
}
