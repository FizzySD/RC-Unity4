using System;
using ExitGames.Client.Photon;
using UnityEngine;

internal class RCAction
{
	public enum actionClasses
	{
		typeVoid = 0,
		typeVariableInt = 1,
		typeVariableBool = 2,
		typeVariableString = 3,
		typeVariableFloat = 4,
		typeVariablePlayer = 5,
		typeVariableTitan = 6,
		typePlayer = 7,
		typeTitan = 8,
		typeGame = 9
	}

	public enum gameTypes
	{
		printMessage = 0,
		winGame = 1,
		loseGame = 2,
		restartGame = 3
	}

	public enum playerTypes
	{
		killPlayer = 0,
		spawnPlayer = 1,
		spawnPlayerAt = 2,
		movePlayer = 3,
		setKills = 4,
		setDeaths = 5,
		setMaxDmg = 6,
		setTotalDmg = 7,
		setName = 8,
		setGuildName = 9,
		setTeam = 10,
		setCustomInt = 11,
		setCustomBool = 12,
		setCustomString = 13,
		setCustomFloat = 14
	}

	public enum titanTypes
	{
		killTitan = 0,
		spawnTitan = 1,
		spawnTitanAt = 2,
		setHealth = 3,
		moveTitan = 4
	}

	public enum varTypes
	{
		set = 0,
		add = 1,
		subtract = 2,
		multiply = 3,
		divide = 4,
		modulo = 5,
		power = 6,
		concat = 7,
		append = 8,
		remove = 9,
		replace = 10,
		toOpposite = 11,
		setRandom = 12
	}

	private int actionClass;

	private int actionType;

	private RCEvent nextEvent;

	private RCActionHelper[] parameters;

	public RCAction(int category, int type, RCEvent next, RCActionHelper[] helpers)
	{
		actionClass = category;
		actionType = type;
		nextEvent = next;
		parameters = helpers;
	}

	public void callException(string str)
	{
		FengGameManagerMKII.instance.chatRoom.addLINE(str);
	}

	public void doAction()
	{
		switch (actionClass)
		{
		case 0:
			nextEvent.checkEvent();
			break;
		case 1:
		{
			string text5 = parameters[0].returnString(null);
			int num2 = parameters[1].returnInt(null);
			switch (actionType)
			{
			case 0:
				if (!FengGameManagerMKII.intVariables.ContainsKey(text5))
				{
					FengGameManagerMKII.intVariables.Add(text5, num2);
				}
				else
				{
					FengGameManagerMKII.intVariables[text5] = num2;
				}
				break;
			case 1:
				if (!FengGameManagerMKII.intVariables.ContainsKey(text5))
				{
					callException("Variable not found: " + text5);
				}
				else
				{
					FengGameManagerMKII.intVariables[text5] = (int)FengGameManagerMKII.intVariables[text5] + num2;
				}
				break;
			case 2:
				if (!FengGameManagerMKII.intVariables.ContainsKey(text5))
				{
					callException("Variable not found: " + text5);
				}
				else
				{
					FengGameManagerMKII.intVariables[text5] = (int)FengGameManagerMKII.intVariables[text5] - num2;
				}
				break;
			case 3:
				if (!FengGameManagerMKII.intVariables.ContainsKey(text5))
				{
					callException("Variable not found: " + text5);
				}
				else
				{
					FengGameManagerMKII.intVariables[text5] = (int)FengGameManagerMKII.intVariables[text5] * num2;
				}
				break;
			case 4:
				if (!FengGameManagerMKII.intVariables.ContainsKey(text5))
				{
					callException("Variable not found: " + text5);
				}
				else
				{
					FengGameManagerMKII.intVariables[text5] = (int)FengGameManagerMKII.intVariables[text5] / num2;
				}
				break;
			case 5:
				if (!FengGameManagerMKII.intVariables.ContainsKey(text5))
				{
					callException("Variable not found: " + text5);
				}
				else
				{
					FengGameManagerMKII.intVariables[text5] = (int)FengGameManagerMKII.intVariables[text5] % num2;
				}
				break;
			case 6:
				if (!FengGameManagerMKII.intVariables.ContainsKey(text5))
				{
					callException("Variable not found: " + text5);
				}
				else
				{
					FengGameManagerMKII.intVariables[text5] = (int)Math.Pow((int)FengGameManagerMKII.intVariables[text5], num2);
				}
				break;
			case 12:
				if (!FengGameManagerMKII.intVariables.ContainsKey(text5))
				{
					FengGameManagerMKII.intVariables.Add(text5, UnityEngine.Random.Range(num2, parameters[2].returnInt(null)));
				}
				else
				{
					FengGameManagerMKII.intVariables[text5] = UnityEngine.Random.Range(num2, parameters[2].returnInt(null));
				}
				break;
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				break;
			}
			break;
		}
		case 2:
		{
			string text4 = parameters[0].returnString(null);
			bool flag = parameters[1].returnBool(null);
			switch (actionType)
			{
			case 11:
				if (!FengGameManagerMKII.boolVariables.ContainsKey(text4))
				{
					callException("Variable not found: " + text4);
				}
				else
				{
					FengGameManagerMKII.boolVariables[text4] = !(bool)FengGameManagerMKII.boolVariables[text4];
				}
				break;
			case 12:
				if (!FengGameManagerMKII.boolVariables.ContainsKey(text4))
				{
					FengGameManagerMKII.boolVariables.Add(text4, Convert.ToBoolean(UnityEngine.Random.Range(0, 2)));
				}
				else
				{
					FengGameManagerMKII.boolVariables[text4] = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
				}
				break;
			case 0:
				if (!FengGameManagerMKII.boolVariables.ContainsKey(text4))
				{
					FengGameManagerMKII.boolVariables.Add(text4, flag);
				}
				else
				{
					FengGameManagerMKII.boolVariables[text4] = flag;
				}
				break;
			}
			break;
		}
		case 3:
		{
			string key3 = parameters[0].returnString(null);
			switch (actionType)
			{
			case 7:
			{
				string text2 = string.Empty;
				for (int i = 1; i < parameters.Length; i++)
				{
					text2 += parameters[i].returnString(null);
				}
				if (!FengGameManagerMKII.stringVariables.ContainsKey(key3))
				{
					FengGameManagerMKII.stringVariables.Add(key3, text2);
				}
				else
				{
					FengGameManagerMKII.stringVariables[key3] = text2;
				}
				break;
			}
			case 8:
			{
				string text = parameters[1].returnString(null);
				if (!FengGameManagerMKII.stringVariables.ContainsKey(key3))
				{
					callException("No Variable");
				}
				else
				{
					FengGameManagerMKII.stringVariables[key3] = (string)FengGameManagerMKII.stringVariables[key3] + text;
				}
				break;
			}
			case 9:
			{
				string text3 = parameters[1].returnString(null);
				if (!FengGameManagerMKII.stringVariables.ContainsKey(key3))
				{
					callException("No Variable");
				}
				else
				{
					FengGameManagerMKII.stringVariables[key3] = ((string)FengGameManagerMKII.stringVariables[key3]).Replace(parameters[1].returnString(null), parameters[2].returnString(null));
				}
				break;
			}
			case 0:
			{
				string value2 = parameters[1].returnString(null);
				if (!FengGameManagerMKII.stringVariables.ContainsKey(key3))
				{
					FengGameManagerMKII.stringVariables.Add(key3, value2);
				}
				else
				{
					FengGameManagerMKII.stringVariables[key3] = value2;
				}
				break;
			}
			}
			break;
		}
		case 4:
		{
			string key2 = parameters[0].returnString(null);
			float num = parameters[1].returnFloat(null);
			switch (actionType)
			{
			case 0:
				if (!FengGameManagerMKII.floatVariables.ContainsKey(key2))
				{
					FengGameManagerMKII.floatVariables.Add(key2, num);
				}
				else
				{
					FengGameManagerMKII.floatVariables[key2] = num;
				}
				break;
			case 1:
				if (!FengGameManagerMKII.floatVariables.ContainsKey(key2))
				{
					callException("No Variable");
				}
				else
				{
					FengGameManagerMKII.floatVariables[key2] = (float)FengGameManagerMKII.floatVariables[key2] + num;
				}
				break;
			case 2:
				if (!FengGameManagerMKII.floatVariables.ContainsKey(key2))
				{
					callException("No Variable");
				}
				else
				{
					FengGameManagerMKII.floatVariables[key2] = (float)FengGameManagerMKII.floatVariables[key2] - num;
				}
				break;
			case 3:
				if (!FengGameManagerMKII.floatVariables.ContainsKey(key2))
				{
					callException("No Variable");
				}
				else
				{
					FengGameManagerMKII.floatVariables[key2] = (float)FengGameManagerMKII.floatVariables[key2] * num;
				}
				break;
			case 4:
				if (!FengGameManagerMKII.floatVariables.ContainsKey(key2))
				{
					callException("No Variable");
				}
				else
				{
					FengGameManagerMKII.floatVariables[key2] = (float)FengGameManagerMKII.floatVariables[key2] / num;
				}
				break;
			case 5:
				if (!FengGameManagerMKII.floatVariables.ContainsKey(key2))
				{
					callException("No Variable");
				}
				else
				{
					FengGameManagerMKII.floatVariables[key2] = (float)FengGameManagerMKII.floatVariables[key2] % num;
				}
				break;
			case 6:
				if (!FengGameManagerMKII.floatVariables.ContainsKey(key2))
				{
					callException("No Variable");
				}
				else
				{
					FengGameManagerMKII.floatVariables[key2] = (float)Math.Pow((int)FengGameManagerMKII.floatVariables[key2], num);
				}
				break;
			case 12:
				if (!FengGameManagerMKII.floatVariables.ContainsKey(key2))
				{
					FengGameManagerMKII.floatVariables.Add(key2, UnityEngine.Random.Range(num, parameters[2].returnFloat(null)));
				}
				else
				{
					FengGameManagerMKII.floatVariables[key2] = UnityEngine.Random.Range(num, parameters[2].returnFloat(null));
				}
				break;
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				break;
			}
			break;
		}
		case 5:
		{
			string key4 = parameters[0].returnString(null);
			PhotonPlayer value3 = parameters[1].returnPlayer(null);
			if (actionType == 0)
			{
				if (!FengGameManagerMKII.playerVariables.ContainsKey(key4))
				{
					FengGameManagerMKII.playerVariables.Add(key4, value3);
				}
				else
				{
					FengGameManagerMKII.playerVariables[key4] = value3;
				}
			}
			break;
		}
		case 6:
		{
			string key = parameters[0].returnString(null);
			TITAN value = parameters[1].returnTitan(null);
			if (actionType == 0)
			{
				if (!FengGameManagerMKII.titanVariables.ContainsKey(key))
				{
					FengGameManagerMKII.titanVariables.Add(key, value);
				}
				else
				{
					FengGameManagerMKII.titanVariables[key] = value;
				}
			}
			break;
		}
		case 7:
		{
			PhotonPlayer photonPlayer = parameters[0].returnPlayer(null);
			switch (actionType)
			{
			case 0:
			{
				int iD2 = photonPlayer.ID;
				if (FengGameManagerMKII.heroHash.ContainsKey(iD2))
				{
					HERO hERO2 = (HERO)FengGameManagerMKII.heroHash[iD2];
					hERO2.markDie();
					hERO2.photonView.RPC("netDie2", PhotonTargets.All, -1, parameters[1].returnString(null) + " ");
				}
				else
				{
					callException("Player Not Alive");
				}
				break;
			}
			case 1:
				FengGameManagerMKII.instance.photonView.RPC("respawnHeroInNewRound", photonPlayer);
				break;
			case 2:
				FengGameManagerMKII.instance.photonView.RPC("spawnPlayerAtRPC", photonPlayer, parameters[1].returnFloat(null), parameters[2].returnFloat(null), parameters[3].returnFloat(null));
				break;
			case 3:
			{
				int iD = photonPlayer.ID;
				if (FengGameManagerMKII.heroHash.ContainsKey(iD))
				{
					HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD];
					hERO.photonView.RPC("moveToRPC", photonPlayer, parameters[1].returnFloat(null), parameters[2].returnFloat(null), parameters[3].returnFloat(null));
				}
				else
				{
					callException("Player Not Alive");
				}
				break;
			}
			case 4:
			{
				Hashtable hashtable11 = new Hashtable();
				hashtable11.Add(PhotonPlayerProperty.kills, parameters[1].returnInt(null));
				photonPlayer.SetCustomProperties(hashtable11);
				break;
			}
			case 5:
			{
				Hashtable hashtable10 = new Hashtable();
				hashtable10.Add(PhotonPlayerProperty.deaths, parameters[1].returnInt(null));
				photonPlayer.SetCustomProperties(hashtable10);
				break;
			}
			case 6:
			{
				Hashtable hashtable9 = new Hashtable();
				hashtable9.Add(PhotonPlayerProperty.max_dmg, parameters[1].returnInt(null));
				photonPlayer.SetCustomProperties(hashtable9);
				break;
			}
			case 7:
			{
				Hashtable hashtable8 = new Hashtable();
				hashtable8.Add(PhotonPlayerProperty.total_dmg, parameters[1].returnInt(null));
				photonPlayer.SetCustomProperties(hashtable8);
				break;
			}
			case 8:
			{
				Hashtable hashtable7 = new Hashtable();
				hashtable7.Add(PhotonPlayerProperty.name, parameters[1].returnString(null));
				photonPlayer.SetCustomProperties(hashtable7);
				break;
			}
			case 9:
			{
				Hashtable hashtable6 = new Hashtable();
				hashtable6.Add(PhotonPlayerProperty.guildName, parameters[1].returnString(null));
				photonPlayer.SetCustomProperties(hashtable6);
				break;
			}
			case 10:
			{
				Hashtable hashtable5 = new Hashtable();
				hashtable5.Add(PhotonPlayerProperty.RCteam, parameters[1].returnInt(null));
				photonPlayer.SetCustomProperties(hashtable5);
				break;
			}
			case 11:
			{
				Hashtable hashtable4 = new Hashtable();
				hashtable4.Add(PhotonPlayerProperty.customInt, parameters[1].returnInt(null));
				photonPlayer.SetCustomProperties(hashtable4);
				break;
			}
			case 12:
			{
				Hashtable hashtable3 = new Hashtable();
				hashtable3.Add(PhotonPlayerProperty.customBool, parameters[1].returnBool(null));
				photonPlayer.SetCustomProperties(hashtable3);
				break;
			}
			case 13:
			{
				Hashtable hashtable2 = new Hashtable();
				hashtable2.Add(PhotonPlayerProperty.customString, parameters[1].returnString(null));
				photonPlayer.SetCustomProperties(hashtable2);
				break;
			}
			case 14:
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add(PhotonPlayerProperty.RCteam, parameters[1].returnFloat(null));
				photonPlayer.SetCustomProperties(hashtable);
				break;
			}
			}
			break;
		}
		case 8:
			switch (actionType)
			{
			case 0:
			{
				TITAN tITAN3 = parameters[0].returnTitan(null);
				object[] array = new object[2]
				{
					parameters[1].returnPlayer(null).ID,
					parameters[2].returnInt(null)
				};
				tITAN3.photonView.RPC("titanGetHit", tITAN3.photonView.owner, array);
				break;
			}
			case 1:
				FengGameManagerMKII.instance.spawnTitanAction(parameters[0].returnInt(null), parameters[1].returnFloat(null), parameters[2].returnInt(null), parameters[3].returnInt(null));
				break;
			case 2:
				FengGameManagerMKII.instance.spawnTitanAtAction(parameters[0].returnInt(null), parameters[1].returnFloat(null), parameters[2].returnInt(null), parameters[3].returnInt(null), parameters[4].returnFloat(null), parameters[5].returnFloat(null), parameters[6].returnFloat(null));
				break;
			case 3:
			{
				TITAN tITAN2 = parameters[0].returnTitan(null);
				int currentHealth = parameters[1].returnInt(null);
				tITAN2.currentHealth = currentHealth;
				if (tITAN2.maxHealth == 0)
				{
					tITAN2.maxHealth = tITAN2.currentHealth;
				}
				tITAN2.photonView.RPC("labelRPC", PhotonTargets.AllBuffered, tITAN2.currentHealth, tITAN2.maxHealth);
				break;
			}
			case 4:
			{
				TITAN tITAN = parameters[0].returnTitan(null);
				if (tITAN.photonView.isMine)
				{
					tITAN.moveTo(parameters[1].returnFloat(null), parameters[2].returnFloat(null), parameters[3].returnFloat(null));
					break;
				}
				tITAN.photonView.RPC("moveToRPC", tITAN.photonView.owner, parameters[1].returnFloat(null), parameters[2].returnFloat(null), parameters[3].returnFloat(null));
				break;
			}
			}
			break;
		case 9:
			switch (actionType)
			{
			case 0:
				FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, parameters[0].returnString(null), string.Empty);
				break;
			case 1:
				FengGameManagerMKII.instance.gameWin2();
				if (parameters[0].returnBool(null))
				{
					FengGameManagerMKII.intVariables.Clear();
					FengGameManagerMKII.boolVariables.Clear();
					FengGameManagerMKII.stringVariables.Clear();
					FengGameManagerMKII.floatVariables.Clear();
					FengGameManagerMKII.playerVariables.Clear();
					FengGameManagerMKII.titanVariables.Clear();
				}
				break;
			case 2:
				FengGameManagerMKII.instance.gameLose2();
				if (parameters[0].returnBool(null))
				{
					FengGameManagerMKII.intVariables.Clear();
					FengGameManagerMKII.boolVariables.Clear();
					FengGameManagerMKII.stringVariables.Clear();
					FengGameManagerMKII.floatVariables.Clear();
					FengGameManagerMKII.playerVariables.Clear();
					FengGameManagerMKII.titanVariables.Clear();
				}
				break;
			case 3:
				if (parameters[0].returnBool(null))
				{
					FengGameManagerMKII.intVariables.Clear();
					FengGameManagerMKII.boolVariables.Clear();
					FengGameManagerMKII.stringVariables.Clear();
					FengGameManagerMKII.floatVariables.Clear();
					FengGameManagerMKII.playerVariables.Clear();
					FengGameManagerMKII.titanVariables.Clear();
				}
				FengGameManagerMKII.instance.restartGame2();
				break;
			}
			break;
		}
	}
}
