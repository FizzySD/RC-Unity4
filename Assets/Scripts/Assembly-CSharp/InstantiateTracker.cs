using Settings;
using UnityEngine;

public class InstantiateTracker
{
	private class AhssShots : ThingToCheck
	{
		private float lastShot = Time.time;

		private int shots = 1;

		public AhssShots()
		{
			type = GameResource.shotGun;
		}

		public override bool KickWorthy()
		{
			if (Time.time - lastShot < 1f)
			{
				shots++;
				if (shots > 2)
				{
					return true;
				}
			}
			else
			{
				shots = 0;
			}
			lastShot = Time.time;
			return false;
		}

		public override void reset()
		{
		}
	}

	private class BladeHitEffect : ThingToCheck
	{
		private float accumTime;

		private float lastHit = Time.time;

		public BladeHitEffect()
		{
			type = GameResource.bladeHit;
		}

		public override bool KickWorthy()
		{
			float num = Time.time - lastHit;
			lastHit = Time.time;
			if (num <= 0.3f)
			{
				accumTime += num;
				return accumTime >= 1.25f;
			}
			accumTime = 0f;
			return false;
		}

		public override void reset()
		{
		}
	}

	private class BloodEffect : ThingToCheck
	{
		private float accumTime;

		private float lastHit = Time.time;

		public BloodEffect()
		{
			type = GameResource.bloodEffect;
		}

		public override bool KickWorthy()
		{
			float num = Time.time - lastHit;
			lastHit = Time.time;
			if (num <= 0.3f)
			{
				accumTime += num;
				return accumTime >= 2f;
			}
			accumTime = 0f;
			return false;
		}

		public override void reset()
		{
		}
	}

	private class ExcessiveBombs : ThingToCheck
	{
		private int count = 1;

		private float lastClear = Time.time;

		public ExcessiveBombs()
		{
			type = GameResource.bomb;
		}

		public override bool KickWorthy()
		{
			if (Time.time - lastClear > 5f)
			{
				count = 0;
				lastClear = Time.time;
			}
			count++;
			return count > 20;
		}

		public override void reset()
		{
		}
	}

	private class ExcessiveEffect : ThingToCheck
	{
		private int effectCounter = 1;

		private float lastEffectTime = Time.time;

		public ExcessiveEffect()
		{
			type = GameResource.effect;
		}

		public override bool KickWorthy()
		{
			if (Time.time - lastEffectTime >= 2f)
			{
				effectCounter = 0;
				lastEffectTime = Time.time;
			}
			effectCounter++;
			return effectCounter > 8;
		}

		public override void reset()
		{
		}
	}

	private class ExcessiveFlares : ThingToCheck
	{
		private int flares = 1;

		private float lastFlare = Time.time;

		public ExcessiveFlares()
		{
			type = GameResource.flare;
		}

		public override bool KickWorthy()
		{
			if (Time.time - lastFlare >= 10f)
			{
				flares = 0;
				lastFlare = Time.time;
			}
			flares++;
			return flares > 4;
		}

		public override void reset()
		{
		}
	}

	private class ExcessiveNameChange : ThingToCheck
	{
		private float lastNameChange = Time.time;

		private int nameChanges = 1;

		public ExcessiveNameChange()
		{
			type = GameResource.name;
		}

		public override bool KickWorthy()
		{
			float num = Time.time - lastNameChange;
			lastNameChange = Time.time;
			if (num >= 5f)
			{
				nameChanges = 0;
			}
			nameChanges++;
			return nameChanges > 5;
		}

		public override void reset()
		{
			nameChanges = 0;
			lastNameChange = Time.time;
		}
	}

	public enum GameResource
	{
		none = 0,
		shotGun = 1,
		effect = 2,
		flare = 3,
		bladeHit = 4,
		bloodEffect = 5,
		general = 6,
		name = 7,
		bomb = 8
	}

	private class GenerallyExcessive : ThingToCheck
	{
		private int count = 1;

		private float lastClear = Time.time;

		public GenerallyExcessive()
		{
			type = GameResource.general;
		}

		public override bool KickWorthy()
		{
			if (Time.time - lastClear > 5f)
			{
				count = 0;
				lastClear = Time.time;
			}
			count++;
			return count > 35;
		}

		public override void reset()
		{
		}
	}

	private class Player
	{
		public int id;

		private ThingToCheck[] thingsToCheck;

		public Player(int id)
		{
			this.id = id;
			thingsToCheck = new ThingToCheck[0];
		}

		private ThingToCheck GameResourceToThing(GameResource gr)
		{
			switch (gr)
			{
			case GameResource.shotGun:
				return new AhssShots();
			case GameResource.effect:
				return new ExcessiveEffect();
			case GameResource.flare:
				return new ExcessiveFlares();
			case GameResource.bladeHit:
				return new BladeHitEffect();
			case GameResource.bloodEffect:
				return new BloodEffect();
			case GameResource.general:
				return new GenerallyExcessive();
			case GameResource.name:
				return new ExcessiveNameChange();
			case GameResource.bomb:
				return new ExcessiveBombs();
			default:
				return null;
			}
		}

		private int GetThingToCheck(GameResource type)
		{
			for (int i = 0; i < thingsToCheck.Length; i++)
			{
				if (thingsToCheck[i].type == type)
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsThingExcessive(GameResource gr)
		{
			int thingToCheck = GetThingToCheck(gr);
			if (thingToCheck > -1)
			{
				return thingsToCheck[thingToCheck].KickWorthy();
			}
			RCextensions.Add(ref thingsToCheck, GameResourceToThing(gr));
			return false;
		}

		public void resetNameTracking()
		{
			int thingToCheck = GetThingToCheck(GameResource.name);
			if (thingToCheck > -1)
			{
				thingsToCheck[thingToCheck].reset();
			}
		}
	}

	private abstract class ThingToCheck
	{
		public GameResource type;

		public abstract bool KickWorthy();

		public abstract void reset();
	}

	public static readonly InstantiateTracker instance = new InstantiateTracker();

	private Player[] players = new Player[0];

	public bool checkObj(string key, PhotonPlayer photonPlayer, int[] viewIDS)
	{
		if (photonPlayer.isMasterClient || photonPlayer.isLocal)
		{
			return true;
		}
		int num = photonPlayer.ID * PhotonNetwork.MAX_VIEW_IDS;
		int num2 = num + PhotonNetwork.MAX_VIEW_IDS;
		foreach (int num3 in viewIDS)
		{
			if (num3 <= num || num3 >= num2)
			{
				if (PhotonNetwork.isMasterClient)
				{
					FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning invalid photon view.");
				}
				return false;
			}
		}
		key = key.ToLower();
		switch (key)
		{
		case "rcasset/bombmain":
		case "rcasset/bombexplodemain":
			if (!SettingsManager.LegacyGameSettings.BombModeEnabled.Value)
			{
				if (PhotonNetwork.isMasterClient && !FengGameManagerMKII.instance.restartingBomb)
				{
					FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning bomb item (" + key + ").");
				}
				return false;
			}
			return Instantiated(photonPlayer, GameResource.bomb);
		case "hook":
		case "aottg_hero 1":
			return Instantiated(photonPlayer, GameResource.general);
		case "hitmeat2":
			return Instantiated(photonPlayer, GameResource.bloodEffect);
		case "hitmeat":
		case "redcross":
		case "redcross1":
			return Instantiated(photonPlayer, GameResource.bladeHit);
		case "fx/flarebullet1":
		case "fx/flarebullet2":
		case "fx/flarebullet3":
			return Instantiated(photonPlayer, GameResource.flare);
		case "fx/shotgun":
		case "fx/shotgun 1":
			return Instantiated(photonPlayer, GameResource.shotGun);
		case "fx/fxtitanspawn":
		case "fx/boom1":
		case "fx/boom3":
		case "fx/boom5":
		case "fx/rockthrow":
		case "fx/bite":
			if (LevelInfo.getInfo(FengGameManagerMKII.level).teamTitan || SettingsManager.LegacyGameSettings.InfectionModeEnabled.Value || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
			{
				return Instantiated(photonPlayer, GameResource.effect);
			}
			if (PhotonNetwork.isMasterClient && !FengGameManagerMKII.instance.restartingTitan)
			{
				FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, false, "spawning titan effects.");
			}
			return false;
		case "fx/boom2":
		case "fx/boom4":
		case "fx/fxtitandie":
		case "fx/fxtitandie1":
		case "fx/boost_smoke":
		case "fx/thunder":
			return Instantiated(photonPlayer, GameResource.effect);
		case "rcasset/cannonballobject":
			return Instantiated(photonPlayer, GameResource.bomb);
		case "rcasset/cannonwall":
		case "rcasset/cannonground":
			if (PhotonNetwork.isMasterClient && !FengGameManagerMKII.instance.allowedToCannon.ContainsKey(photonPlayer.ID) && !FengGameManagerMKII.instance.restartingMC)
			{
				FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, false, "spawning cannon item (" + key + ").");
			}
			return Instantiated(photonPlayer, GameResource.general);
		case "rcasset/cannonwallprop":
		case "rcasset/cannongroundprop":
			if (PhotonNetwork.isMasterClient)
			{
				FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning MC item (" + key + ").");
			}
			return false;
		case "titan_eren":
			if (!(RCextensions.returnStringFromObject(photonPlayer.customProperties[PhotonPlayerProperty.character]).ToUpper() != "EREN"))
			{
				if (SettingsManager.LegacyGameSettings.KickShifters.Value)
				{
					if (PhotonNetwork.isMasterClient && !FengGameManagerMKII.instance.restartingEren)
					{
						FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, false, "spawning titan eren (" + key + ").");
					}
					return false;
				}
				return Instantiated(photonPlayer, GameResource.general);
			}
			if (PhotonNetwork.isMasterClient)
			{
				FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning titan eren (" + key + ").");
			}
			return false;
		case "fx/justSmoke":
		case "bloodexplore":
		case "bloodsplatter":
			return Instantiated(photonPlayer, GameResource.effect);
		case "hitmeatbig":
			if (!(RCextensions.returnStringFromObject(photonPlayer.customProperties[PhotonPlayerProperty.character]).ToUpper() != "EREN"))
			{
				if (SettingsManager.LegacyGameSettings.KickShifters.Value)
				{
					if (PhotonNetwork.isMasterClient && !FengGameManagerMKII.instance.restartingEren)
					{
						FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, false, "spawning eren effect (" + key + ").");
					}
					return false;
				}
				return Instantiated(photonPlayer, GameResource.effect);
			}
			if (PhotonNetwork.isMasterClient)
			{
				FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning eren effect (" + key + ").");
			}
			return false;
		case "fx/colossal_steam_dmg":
		case "fx/colossal_steam":
		case "fx/boom1_ct_kick":
			if (!PhotonNetwork.isMasterClient || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
			{
				return Instantiated(photonPlayer, GameResource.effect);
			}
			FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning colossal effect (" + key + ").");
			return false;
		case "rock":
			if (!PhotonNetwork.isMasterClient || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
			{
				return Instantiated(photonPlayer, GameResource.general);
			}
			FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning MC item (" + key + ").");
			return false;
		case "horse":
			if (LevelInfo.getInfo(FengGameManagerMKII.level).horse || SettingsManager.LegacyGameSettings.AllowHorses.Value)
			{
				return Instantiated(photonPlayer, GameResource.general);
			}
			if (PhotonNetwork.isMasterClient && !FengGameManagerMKII.instance.restartingHorse)
			{
				FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning horse (" + key + ").");
			}
			return false;
		case "titan_ver3.1":
			if (!PhotonNetwork.isMasterClient)
			{
				if (FengGameManagerMKII.masterRC && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.BOSS_FIGHT_CT)
				{
					int num4 = 0;
					foreach (TITAN titan in FengGameManagerMKII.instance.getTitans())
					{
						if (titan.photonView.owner == photonPlayer)
						{
							num4++;
						}
					}
					if (num4 > 1)
					{
						return false;
					}
				}
			}
			else
			{
				if (!LevelInfo.getInfo(FengGameManagerMKII.level).teamTitan && !SettingsManager.LegacyGameSettings.InfectionModeEnabled.Value && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.BOSS_FIGHT_CT && !FengGameManagerMKII.instance.restartingTitan)
				{
					FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, false, "spawning titan (" + key + ").");
					return false;
				}
				if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.BOSS_FIGHT_CT)
				{
					int num4 = 0;
					foreach (TITAN titan2 in FengGameManagerMKII.instance.getTitans())
					{
						if (titan2.photonView.owner == photonPlayer)
						{
							num4++;
						}
					}
					if (num4 > 1)
					{
						FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, false, "spawning titan (" + key + ").");
						return false;
					}
				}
			}
			return Instantiated(photonPlayer, GameResource.general);
		case "colossal_titan":
		case "female_titan":
		case "titan_eren_trost":
		case "aot_supply":
		case "monsterprefab":
		case "titan_new_1":
		case "titan_new_2":
			if (!PhotonNetwork.isMasterClient)
			{
				if (FengGameManagerMKII.masterRC)
				{
					return false;
				}
				return Instantiated(photonPlayer, GameResource.general);
			}
			FengGameManagerMKII.instance.kickPlayerRC(photonPlayer, true, "spawning MC item (" + key + ").");
			return false;
		default:
			return false;
		}
	}

	public void Dispose()
	{
		players = null;
		players = new Player[0];
	}

	public bool Instantiated(PhotonPlayer owner, GameResource type)
	{
		int result;
		if (TryGetPlayer(owner.ID, out result))
		{
			if (players[result].IsThingExcessive(type))
			{
				if (owner != null && PhotonNetwork.isMasterClient)
				{
					FengGameManagerMKII.instance.kickPlayerRC(owner, true, "spamming instantiate (" + type.ToString() + ").");
				}
				RCextensions.RemoveAt(ref players, result);
				return false;
			}
		}
		else
		{
			RCextensions.Add(ref players, new Player(owner.ID));
			players[players.Length - 1].IsThingExcessive(type);
		}
		return true;
	}

	public bool PropertiesChanged(PhotonPlayer owner)
	{
		int result;
		if (TryGetPlayer(owner.ID, out result))
		{
			if (players[result].IsThingExcessive(GameResource.name))
			{
				return false;
			}
		}
		else
		{
			RCextensions.Add(ref players, new Player(owner.ID));
			players[players.Length - 1].IsThingExcessive(GameResource.name);
		}
		return true;
	}

	public void resetPropertyTracking(int ID)
	{
		int result;
		if (TryGetPlayer(ID, out result))
		{
			players[result].resetNameTracking();
		}
	}

	private bool TryGetPlayer(int id, out int result)
	{
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].id == id)
			{
				result = i;
				return true;
			}
		}
		result = -1;
		return false;
	}

	public void TryRemovePlayer(int playerId)
	{
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].id == playerId)
			{
				RCextensions.RemoveAt(ref players, i);
				break;
			}
		}
	}
}
