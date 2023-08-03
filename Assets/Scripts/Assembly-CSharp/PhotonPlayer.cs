using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer
{
	private int actorID;

	public readonly bool isLocal;

	private string nameField;

	public object TagObject;

	public Hashtable allProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(customProperties);
			hashtable[byte.MaxValue] = name;
			return hashtable;
		}
	}

	public Hashtable customProperties { get; private set; }

	public int ID
	{
		get
		{
			return actorID;
		}
	}

	public bool isMasterClient
	{
		get
		{
			return PhotonNetwork.networkingPeer.mMasterClient == this;
		}
	}

	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			if (!isLocal)
			{
				Debug.LogError("Error: Cannot change the name of a remote player!");
			}
			else
			{
				nameField = value;
			}
		}
	}

	public static void CleanProperties()
	{
		if (PhotonNetwork.player != null)
		{
			PhotonNetwork.player.customProperties.Clear();
			PhotonNetwork.player.SetCustomProperties(new Hashtable { 
			{
				PhotonPlayerProperty.name,
				LoginFengKAI.player.name
			} });
		}
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		this.actorID = -1;
		nameField = string.Empty;
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		InternalCacheProperties(properties);
	}

	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		this.actorID = -1;
		nameField = string.Empty;
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		nameField = name;
	}

	public override bool Equals(object p)
	{
		PhotonPlayer photonPlayer = p as PhotonPlayer;
		if (photonPlayer != null)
		{
			return GetHashCode() == photonPlayer.GetHashCode();
		}
		return false;
	}

	public static PhotonPlayer Find(int ID)
	{
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			if (photonPlayer.ID == ID)
			{
				return photonPlayer;
			}
		}
		return null;
	}

	public Hashtable ChangeLocalPlayer(int NewID, string inputname)
	{
		actorID = NewID;
		string value = (nameField = string.Format("{0}_ID:{1}", inputname, NewID));
		Hashtable hashtable = new Hashtable();
		hashtable.Add(PhotonPlayerProperty.name, value);
		hashtable.Add(PhotonPlayerProperty.guildName, LoginFengKAI.player.guildname);
		hashtable.Add(PhotonPlayerProperty.kills, 0);
		hashtable.Add(PhotonPlayerProperty.max_dmg, 0);
		hashtable.Add(PhotonPlayerProperty.total_dmg, 0);
		hashtable.Add(PhotonPlayerProperty.deaths, 0);
		hashtable.Add(PhotonPlayerProperty.dead, true);
		hashtable.Add(PhotonPlayerProperty.isTitan, 0);
		hashtable.Add(PhotonPlayerProperty.RCteam, 0);
		hashtable.Add(PhotonPlayerProperty.currentLevel, string.Empty);
		Hashtable result = hashtable;
		PhotonNetwork.AllocateViewID();
		PhotonNetwork.player.SetCustomProperties(result);
		return result;
	}

	public PhotonPlayer Get(int id)
	{
		return Find(id);
	}

	public override int GetHashCode()
	{
		return ID;
	}

	public PhotonPlayer GetNext()
	{
		return GetNextFor(ID);
	}

	public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		return GetNextFor(currentPlayer.ID);
	}

	public PhotonPlayer GetNextFor(int currentPlayerId)
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
		{
			return null;
		}
		Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
		int num = int.MaxValue;
		int num2 = currentPlayerId;
		foreach (int key in mActors.Keys)
		{
			if (key < num2)
			{
				num2 = key;
			}
			else if (key > currentPlayerId && key < num)
			{
				num = key;
			}
		}
		if (num != int.MaxValue)
		{
			return mActors[num];
		}
		return mActors[num2];
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties != null && properties.Count != 0 && !customProperties.Equals(properties))
		{
			if (properties.ContainsKey(byte.MaxValue))
			{
				nameField = (string)properties[byte.MaxValue];
			}
			customProperties.MergeStringKeys(properties);
			customProperties.StripKeysWithNullValues();
		}
	}

	internal void InternalChangeLocalID(int newID)
	{
		if (!isLocal)
		{
			Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
		}
		else
		{
			actorID = newID;
		}
	}

	public void SetCustomProperties(Hashtable propertiesToSet)
	{
		if (propertiesToSet != null)
		{
			customProperties.MergeStringKeys(propertiesToSet);
			customProperties.StripKeysWithNullValues();
			Hashtable actorProperties = propertiesToSet.StripToStringKeys();
			if (actorID > 0 && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(actorID, actorProperties, true, 0);
			}
			object[] parameters = new object[2] { this, propertiesToSet };
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, parameters);
		}
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(name))
		{
			return string.Format("#{0:00}{1}", ID, (!isMasterClient) ? string.Empty : "(master)");
		}
		return string.Format("'{0}'{1}", name, (!isMasterClient) ? string.Empty : "(master)");
	}

	public string ToStringFull()
	{
		return string.Format("#{0:00} '{1}' {2}", ID, name, customProperties.ToStringFull());
	}
}
