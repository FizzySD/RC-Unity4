using UnityEngine;

public class PlayerInfoPHOTON
{
	public int airKills;

	public int assistancePt;

	public bool dead;

	public int die;

	public string guildname = string.Empty;

	public string id;

	public int kills;

	public int maxDamage;

	public string name = "Guest";

	public PhotonPlayer networkplayer;

	public string resourceId = "not choose";

	public bool SET;

	public int totalCrawlerKills;

	public int totalDamage;

	public int totalDeaths;

	public int totalJumperKills;

	public int totalKills;

	public int totalKillsInOneLifeAB;

	public int totalKillsInOneLifeHard;

	public int totalKillsInOneLifeNormal;

	public int totalNonAIKills;

	public void initAsGuest()
	{
		name = "GUEST" + Random.Range(0, 100000);
		kills = 0;
		die = 0;
		maxDamage = 0;
		totalDamage = 0;
		assistancePt = 0;
		dead = false;
		resourceId = "not choose";
		SET = false;
		totalKills = 0;
		totalDeaths = 0;
		totalKillsInOneLifeNormal = 0;
		totalKillsInOneLifeHard = 0;
		totalKillsInOneLifeAB = 0;
		airKills = 0;
		totalCrawlerKills = 0;
		totalJumperKills = 0;
		totalNonAIKills = 0;
	}
}
