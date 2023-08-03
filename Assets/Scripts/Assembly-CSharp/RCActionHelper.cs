using System;

internal class RCActionHelper
{
	public enum helperClasses
	{
		primitive = 0,
		variable = 1,
		player = 2,
		titan = 3,
		region = 4,
		convert = 5
	}

	public enum mathTypes
	{
		add = 0,
		subtract = 1,
		multiply = 2,
		divide = 3,
		modulo = 4,
		power = 5
	}

	public enum other
	{
		regionX = 0,
		regionY = 1,
		regionZ = 2
	}

	public enum playerTypes
	{
		playerType = 0,
		playerTeam = 1,
		playerAlive = 2,
		playerTitan = 3,
		playerKills = 4,
		playerDeaths = 5,
		playerMaxDamage = 6,
		playerTotalDamage = 7,
		playerCustomInt = 8,
		playerCustomBool = 9,
		playerCustomString = 10,
		playerCustomFloat = 11,
		playerName = 12,
		playerGuildName = 13,
		playerPosX = 14,
		playerPosY = 15,
		playerPosZ = 16,
		playerSpeed = 17
	}

	public enum titanTypes
	{
		titanType = 0,
		titanSize = 1,
		titanHealth = 2,
		positionX = 3,
		positionY = 4,
		positionZ = 5
	}

	public enum variableTypes
	{
		typeInt = 0,
		typeBool = 1,
		typeString = 2,
		typeFloat = 3,
		typePlayer = 4,
		typeTitan = 5
	}

	public int helperClass;

	public int helperType;

	private RCActionHelper nextHelper;

	private object parameters;

	public RCActionHelper(int sentClass, int sentType, object options)
	{
		helperClass = sentClass;
		helperType = sentType;
		parameters = options;
	}

	public void callException(string str)
	{
		FengGameManagerMKII.instance.chatRoom.addLINE(str);
	}

	public bool returnBool(object sentObject)
	{
		object obj = sentObject;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 0:
			return (bool)obj;
		case 1:
		{
			RCActionHelper rCActionHelper2 = (RCActionHelper)obj;
			switch (helperType)
			{
			case 0:
				return nextHelper.returnBool(FengGameManagerMKII.intVariables[rCActionHelper2.returnString(null)]);
			case 1:
				return (bool)FengGameManagerMKII.boolVariables[rCActionHelper2.returnString(null)];
			case 2:
				return nextHelper.returnBool(FengGameManagerMKII.stringVariables[rCActionHelper2.returnString(null)]);
			case 3:
				return nextHelper.returnBool(FengGameManagerMKII.floatVariables[rCActionHelper2.returnString(null)]);
			case 4:
				return nextHelper.returnBool(FengGameManagerMKII.playerVariables[rCActionHelper2.returnString(null)]);
			case 5:
				return nextHelper.returnBool(FengGameManagerMKII.titanVariables[rCActionHelper2.returnString(null)]);
			default:
				return false;
			}
		}
		case 2:
		{
			PhotonPlayer photonPlayer = (PhotonPlayer)obj;
			if (photonPlayer != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.team]);
				case 1:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.RCteam]);
				case 2:
					return !(bool)photonPlayer.customProperties[PhotonPlayerProperty.dead];
				case 3:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.isTitan]);
				case 4:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.kills]);
				case 5:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.deaths]);
				case 6:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.max_dmg]);
				case 7:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.total_dmg]);
				case 8:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.customInt]);
				case 9:
					return (bool)photonPlayer.customProperties[PhotonPlayerProperty.customBool];
				case 10:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.customString]);
				case 11:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.customFloat]);
				case 12:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.name]);
				case 13:
					return nextHelper.returnBool(photonPlayer.customProperties[PhotonPlayerProperty.guildName]);
				case 14:
				{
					int iD4 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD4))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD4];
						return nextHelper.returnBool(hERO.transform.position.x);
					}
					return false;
				}
				case 15:
				{
					int iD3 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD3))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD3];
						return nextHelper.returnBool(hERO.transform.position.y);
					}
					return false;
				}
				case 16:
				{
					int iD2 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD2))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD2];
						return nextHelper.returnBool(hERO.transform.position.z);
					}
					return false;
				}
				case 17:
				{
					int iD = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD];
						return nextHelper.returnBool(hERO.rigidbody.velocity.magnitude);
					}
					return false;
				}
				}
			}
			return false;
		}
		case 3:
		{
			TITAN tITAN = (TITAN)obj;
			if (tITAN != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.returnBool(tITAN.abnormalType);
				case 1:
					return nextHelper.returnBool(tITAN.myLevel);
				case 2:
					return nextHelper.returnBool(tITAN.currentHealth);
				case 3:
					return nextHelper.returnBool(tITAN.transform.position.x);
				case 4:
					return nextHelper.returnBool(tITAN.transform.position.y);
				case 5:
					return nextHelper.returnBool(tITAN.transform.position.z);
				}
			}
			return false;
		}
		case 4:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			RCRegion rCRegion = (RCRegion)FengGameManagerMKII.RCRegions[rCActionHelper.returnString(null)];
			switch (helperType)
			{
			case 0:
				return nextHelper.returnBool(rCRegion.GetRandomX());
			case 1:
				return nextHelper.returnBool(rCRegion.GetRandomY());
			case 2:
				return nextHelper.returnBool(rCRegion.GetRandomZ());
			default:
				return false;
			}
		}
		case 5:
			switch (helperType)
			{
			case 0:
			{
				int value3 = (int)obj;
				return Convert.ToBoolean(value3);
			}
			case 1:
				return (bool)obj;
			case 2:
			{
				string value2 = (string)obj;
				return Convert.ToBoolean(value2);
			}
			case 3:
			{
				float value = (float)obj;
				return Convert.ToBoolean(value);
			}
			default:
				return false;
			}
		default:
			return false;
		}
	}

	public float returnFloat(object sentObject)
	{
		object obj = sentObject;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 0:
			return (float)obj;
		case 1:
		{
			RCActionHelper rCActionHelper2 = (RCActionHelper)obj;
			switch (helperType)
			{
			case 0:
				return nextHelper.returnFloat(FengGameManagerMKII.intVariables[rCActionHelper2.returnString(null)]);
			case 1:
				return nextHelper.returnFloat(FengGameManagerMKII.boolVariables[rCActionHelper2.returnString(null)]);
			case 2:
				return nextHelper.returnFloat(FengGameManagerMKII.stringVariables[rCActionHelper2.returnString(null)]);
			case 3:
				return (float)FengGameManagerMKII.floatVariables[rCActionHelper2.returnString(null)];
			case 4:
				return nextHelper.returnFloat(FengGameManagerMKII.playerVariables[rCActionHelper2.returnString(null)]);
			case 5:
				return nextHelper.returnFloat(FengGameManagerMKII.titanVariables[rCActionHelper2.returnString(null)]);
			default:
				return 0f;
			}
		}
		case 2:
		{
			PhotonPlayer photonPlayer = (PhotonPlayer)obj;
			if (photonPlayer != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.team]);
				case 1:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.RCteam]);
				case 2:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.dead]);
				case 3:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.isTitan]);
				case 4:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.kills]);
				case 5:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.deaths]);
				case 6:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.max_dmg]);
				case 7:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.total_dmg]);
				case 8:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.customInt]);
				case 9:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.customBool]);
				case 10:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.customString]);
				case 11:
					return (float)photonPlayer.customProperties[PhotonPlayerProperty.customFloat];
				case 12:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.name]);
				case 13:
					return nextHelper.returnFloat(photonPlayer.customProperties[PhotonPlayerProperty.guildName]);
				case 14:
				{
					int iD4 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD4))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD4];
						return hERO.transform.position.x;
					}
					return 0f;
				}
				case 15:
				{
					int iD3 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD3))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD3];
						return hERO.transform.position.y;
					}
					return 0f;
				}
				case 16:
				{
					int iD2 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD2))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD2];
						return hERO.transform.position.z;
					}
					return 0f;
				}
				case 17:
				{
					int iD = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD];
						return hERO.rigidbody.velocity.magnitude;
					}
					return 0f;
				}
				}
			}
			return 0f;
		}
		case 3:
		{
			TITAN tITAN = (TITAN)obj;
			if (tITAN != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.returnFloat(tITAN.abnormalType);
				case 1:
					return tITAN.myLevel;
				case 2:
					return nextHelper.returnFloat(tITAN.currentHealth);
				case 3:
					return tITAN.transform.position.x;
				case 4:
					return tITAN.transform.position.y;
				case 5:
					return tITAN.transform.position.z;
				}
			}
			return 0f;
		}
		case 4:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			RCRegion rCRegion = (RCRegion)FengGameManagerMKII.RCRegions[rCActionHelper.returnString(null)];
			switch (helperType)
			{
			case 0:
				return rCRegion.GetRandomX();
			case 1:
				return rCRegion.GetRandomY();
			case 2:
				return rCRegion.GetRandomZ();
			default:
				return 0f;
			}
		}
		case 5:
			switch (helperType)
			{
			case 0:
			{
				int value2 = (int)obj;
				return Convert.ToSingle(value2);
			}
			case 1:
			{
				bool value = (bool)obj;
				return Convert.ToSingle(value);
			}
			case 2:
			{
				string text = (string)obj;
				float result;
				if (float.TryParse((string)obj, out result))
				{
					return result;
				}
				return 0f;
			}
			case 3:
				return (float)obj;
			default:
				return (float)obj;
			}
		default:
			return 0f;
		}
	}

	public int returnInt(object sentObject)
	{
		object obj = sentObject;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 0:
			return (int)obj;
		case 1:
		{
			RCActionHelper rCActionHelper2 = (RCActionHelper)obj;
			switch (helperType)
			{
			case 0:
				return (int)FengGameManagerMKII.intVariables[rCActionHelper2.returnString(null)];
			case 1:
				return nextHelper.returnInt(FengGameManagerMKII.boolVariables[rCActionHelper2.returnString(null)]);
			case 2:
				return nextHelper.returnInt(FengGameManagerMKII.stringVariables[rCActionHelper2.returnString(null)]);
			case 3:
				return nextHelper.returnInt(FengGameManagerMKII.floatVariables[rCActionHelper2.returnString(null)]);
			case 4:
				return nextHelper.returnInt(FengGameManagerMKII.playerVariables[rCActionHelper2.returnString(null)]);
			case 5:
				return nextHelper.returnInt(FengGameManagerMKII.titanVariables[rCActionHelper2.returnString(null)]);
			default:
				return 0;
			}
		}
		case 2:
		{
			PhotonPlayer photonPlayer = (PhotonPlayer)obj;
			if (photonPlayer != null)
			{
				switch (helperType)
				{
				case 0:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.team];
				case 1:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.RCteam];
				case 2:
					return nextHelper.returnInt(photonPlayer.customProperties[PhotonPlayerProperty.dead]);
				case 3:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.isTitan];
				case 4:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.kills];
				case 5:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.deaths];
				case 6:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.max_dmg];
				case 7:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.total_dmg];
				case 8:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.customInt];
				case 9:
					return nextHelper.returnInt(photonPlayer.customProperties[PhotonPlayerProperty.customBool]);
				case 10:
					return nextHelper.returnInt(photonPlayer.customProperties[PhotonPlayerProperty.customString]);
				case 11:
					return nextHelper.returnInt(photonPlayer.customProperties[PhotonPlayerProperty.customFloat]);
				case 12:
					return nextHelper.returnInt(photonPlayer.customProperties[PhotonPlayerProperty.name]);
				case 13:
					return nextHelper.returnInt(photonPlayer.customProperties[PhotonPlayerProperty.guildName]);
				case 14:
				{
					int iD4 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD4))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD4];
						return nextHelper.returnInt(hERO.transform.position.x);
					}
					return 0;
				}
				case 15:
				{
					int iD3 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD3))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD3];
						return nextHelper.returnInt(hERO.transform.position.y);
					}
					return 0;
				}
				case 16:
				{
					int iD2 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD2))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD2];
						return nextHelper.returnInt(hERO.transform.position.z);
					}
					return 0;
				}
				case 17:
				{
					int iD = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD];
						return nextHelper.returnInt(hERO.rigidbody.velocity.magnitude);
					}
					return 0;
				}
				}
			}
			return 0;
		}
		case 3:
		{
			TITAN tITAN = (TITAN)obj;
			if (tITAN != null)
			{
				switch (helperType)
				{
				case 0:
					return (int)tITAN.abnormalType;
				case 1:
					return nextHelper.returnInt(tITAN.myLevel);
				case 2:
					return tITAN.currentHealth;
				case 3:
					return nextHelper.returnInt(tITAN.transform.position.x);
				case 4:
					return nextHelper.returnInt(tITAN.transform.position.y);
				case 5:
					return nextHelper.returnInt(tITAN.transform.position.z);
				}
			}
			return 0;
		}
		case 4:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			RCRegion rCRegion = (RCRegion)FengGameManagerMKII.RCRegions[rCActionHelper.returnString(null)];
			switch (helperType)
			{
			case 0:
				return nextHelper.returnInt(rCRegion.GetRandomX());
			case 1:
				return nextHelper.returnInt(rCRegion.GetRandomY());
			case 2:
				return nextHelper.returnInt(rCRegion.GetRandomZ());
			default:
				return 0;
			}
		}
		case 5:
			switch (helperType)
			{
			case 0:
				return (int)obj;
			case 1:
			{
				bool value2 = (bool)obj;
				return Convert.ToInt32(value2);
			}
			case 2:
			{
				string text = (string)obj;
				int result;
				if (int.TryParse((string)obj, out result))
				{
					return result;
				}
				return 0;
			}
			case 3:
			{
				float value = (float)obj;
				return Convert.ToInt32(value);
			}
			default:
				return (int)obj;
			}
		default:
			return 0;
		}
	}

	public PhotonPlayer returnPlayer(object objParameter)
	{
		object obj = objParameter;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 1:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			return (PhotonPlayer)FengGameManagerMKII.playerVariables[rCActionHelper.returnString(null)];
		}
		case 2:
			return (PhotonPlayer)obj;
		default:
			return (PhotonPlayer)obj;
		}
	}

	public string returnString(object sentObject)
	{
		object obj = sentObject;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 0:
			return (string)obj;
		case 1:
		{
			RCActionHelper rCActionHelper2 = (RCActionHelper)obj;
			switch (helperType)
			{
			case 0:
				return nextHelper.returnString(FengGameManagerMKII.intVariables[rCActionHelper2.returnString(null)]);
			case 1:
				return nextHelper.returnString(FengGameManagerMKII.boolVariables[rCActionHelper2.returnString(null)]);
			case 2:
				return (string)FengGameManagerMKII.stringVariables[rCActionHelper2.returnString(null)];
			case 3:
				return nextHelper.returnString(FengGameManagerMKII.floatVariables[rCActionHelper2.returnString(null)]);
			case 4:
				return nextHelper.returnString(FengGameManagerMKII.playerVariables[rCActionHelper2.returnString(null)]);
			case 5:
				return nextHelper.returnString(FengGameManagerMKII.titanVariables[rCActionHelper2.returnString(null)]);
			default:
				return string.Empty;
			}
		}
		case 2:
		{
			PhotonPlayer photonPlayer = (PhotonPlayer)obj;
			if (photonPlayer != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.team]);
				case 1:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.RCteam]);
				case 2:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.dead]);
				case 3:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.isTitan]);
				case 4:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.kills]);
				case 5:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.deaths]);
				case 6:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.max_dmg]);
				case 7:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.total_dmg]);
				case 8:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.customInt]);
				case 9:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.customBool]);
				case 10:
					return (string)photonPlayer.customProperties[PhotonPlayerProperty.customString];
				case 11:
					return nextHelper.returnString(photonPlayer.customProperties[PhotonPlayerProperty.customFloat]);
				case 12:
					return (string)photonPlayer.customProperties[PhotonPlayerProperty.name];
				case 13:
					return (string)photonPlayer.customProperties[PhotonPlayerProperty.guildName];
				case 14:
				{
					int iD4 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD4))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD4];
						return nextHelper.returnString(hERO.transform.position.x);
					}
					return string.Empty;
				}
				case 15:
				{
					int iD3 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD3))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD3];
						return nextHelper.returnString(hERO.transform.position.y);
					}
					return string.Empty;
				}
				case 16:
				{
					int iD2 = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD2))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD2];
						return nextHelper.returnString(hERO.transform.position.z);
					}
					return string.Empty;
				}
				case 17:
				{
					int iD = photonPlayer.ID;
					if (FengGameManagerMKII.heroHash.ContainsKey(iD))
					{
						HERO hERO = (HERO)FengGameManagerMKII.heroHash[iD];
						return nextHelper.returnString(hERO.rigidbody.velocity.magnitude);
					}
					return string.Empty;
				}
				}
			}
			return string.Empty;
		}
		case 3:
		{
			TITAN tITAN = (TITAN)obj;
			if (tITAN != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.returnString(tITAN.abnormalType);
				case 1:
					return nextHelper.returnString(tITAN.myLevel);
				case 2:
					return nextHelper.returnString(tITAN.currentHealth);
				case 3:
					return nextHelper.returnString(tITAN.transform.position.x);
				case 4:
					return nextHelper.returnString(tITAN.transform.position.y);
				case 5:
					return nextHelper.returnString(tITAN.transform.position.z);
				}
			}
			return string.Empty;
		}
		case 4:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			RCRegion rCRegion = (RCRegion)FengGameManagerMKII.RCRegions[rCActionHelper.returnString(null)];
			switch (helperType)
			{
			case 0:
				return nextHelper.returnString(rCRegion.GetRandomX());
			case 1:
				return nextHelper.returnString(rCRegion.GetRandomY());
			case 2:
				return nextHelper.returnString(rCRegion.GetRandomZ());
			default:
				return string.Empty;
			}
		}
		case 5:
			switch (helperType)
			{
			case 0:
				return ((int)obj).ToString();
			case 1:
				return ((bool)obj).ToString();
			case 2:
				return (string)obj;
			case 3:
				return ((float)obj).ToString();
			default:
				return string.Empty;
			}
		default:
			return string.Empty;
		}
	}

	public TITAN returnTitan(object objParameter)
	{
		object obj = objParameter;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 1:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			return (TITAN)FengGameManagerMKII.titanVariables[rCActionHelper.returnString(null)];
		}
		case 3:
			return (TITAN)obj;
		default:
			return (TITAN)obj;
		}
	}

	public void setNextHelper(RCActionHelper sentHelper)
	{
		nextHelper = sentHelper;
	}
}
