using System.Collections.Generic;

public class HeroStat
{
	public int ACL;

	public static HeroStat ARMIN;

	public int BLA;

	public static HeroStat EREN;

	public int GAS;

	public static HeroStat[] heroStats;

	private static bool init;

	public static HeroStat JEAN;

	public static HeroStat LEVI;

	public static HeroStat MARCO;

	public static HeroStat MIKASA;

	public string name;

	public static HeroStat PETRA;

	public static HeroStat SASHA;

	public string skillId = "petra";

	public int SPD;

	public static Dictionary<string, HeroStat> stats;

	public static HeroStat getInfo(string name)
	{
		initDATA();
		return stats[name];
	}

	private static void initDATA()
	{
		if (!init)
		{
			init = true;
			MIKASA = new HeroStat();
			LEVI = new HeroStat();
			ARMIN = new HeroStat();
			MARCO = new HeroStat();
			JEAN = new HeroStat();
			EREN = new HeroStat();
			PETRA = new HeroStat();
			SASHA = new HeroStat();
			MIKASA.name = "MIKASA";
			MIKASA.skillId = "mikasa";
			MIKASA.SPD = 125;
			MIKASA.GAS = 75;
			MIKASA.BLA = 75;
			MIKASA.ACL = 135;
			LEVI.name = "LEVI";
			LEVI.skillId = "levi";
			LEVI.SPD = 95;
			LEVI.GAS = 100;
			LEVI.BLA = 100;
			LEVI.ACL = 150;
			ARMIN.name = "ARMIN";
			ARMIN.skillId = "armin";
			ARMIN.SPD = 75;
			ARMIN.GAS = 150;
			ARMIN.BLA = 125;
			ARMIN.ACL = 85;
			MARCO.name = "MARCO";
			MARCO.skillId = "marco";
			MARCO.SPD = 110;
			MARCO.GAS = 100;
			MARCO.BLA = 115;
			MARCO.ACL = 95;
			JEAN.name = "JEAN";
			JEAN.skillId = "jean";
			JEAN.SPD = 100;
			JEAN.GAS = 150;
			JEAN.BLA = 80;
			JEAN.ACL = 100;
			EREN.name = "EREN";
			EREN.skillId = "eren";
			EREN.SPD = 100;
			EREN.GAS = 90;
			EREN.BLA = 90;
			EREN.ACL = 100;
			PETRA.name = "PETRA";
			PETRA.skillId = "petra";
			PETRA.SPD = 80;
			PETRA.GAS = 110;
			PETRA.BLA = 100;
			PETRA.ACL = 140;
			SASHA.name = "SASHA";
			SASHA.skillId = "sasha";
			SASHA.SPD = 140;
			SASHA.GAS = 100;
			SASHA.BLA = 100;
			SASHA.ACL = 115;
			HeroStat value = new HeroStat
			{
				skillId = "petra",
				SPD = 100,
				GAS = 100,
				BLA = 100,
				ACL = 100
			};
			HeroStat heroStat = new HeroStat();
			SASHA.name = "AHSS";
			heroStat.skillId = "sasha";
			heroStat.SPD = 100;
			heroStat.GAS = 100;
			heroStat.BLA = 100;
			heroStat.ACL = 100;
			stats = new Dictionary<string, HeroStat>();
			stats.Add("MIKASA", MIKASA);
			stats.Add("LEVI", LEVI);
			stats.Add("ARMIN", ARMIN);
			stats.Add("MARCO", MARCO);
			stats.Add("JEAN", JEAN);
			stats.Add("EREN", EREN);
			stats.Add("PETRA", PETRA);
			stats.Add("SASHA", SASHA);
			stats.Add("CUSTOM_DEFAULT", value);
			stats.Add("AHSS", heroStat);
		}
	}
}
