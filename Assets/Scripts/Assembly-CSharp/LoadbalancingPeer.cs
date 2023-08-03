using System.Collections.Generic;
using ExitGames.Client.Photon;

internal class LoadbalancingPeer : PhotonPeer
{
	public LoadbalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType)
		: base(listener, protocolType)
	{
	}

	public virtual bool OpAuthenticate(string appId, string appVersion, string userId, AuthenticationValues authValues, string regionCode)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (authValues != null && authValues.Secret != null)
		{
			dictionary[221] = authValues.Secret;
			return OpCustom(230, dictionary, true, 0);
		}
		dictionary[220] = appVersion;
		dictionary[224] = appId;
		if (!string.IsNullOrEmpty(regionCode))
		{
			dictionary[210] = regionCode;
		}
		if (!string.IsNullOrEmpty(userId))
		{
			dictionary[225] = userId;
		}
		if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
		{
			if (!base.IsEncryptionAvailable)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
				return false;
			}
			dictionary[217] = (byte)authValues.AuthType;
			if (!string.IsNullOrEmpty(authValues.Secret))
			{
				dictionary[221] = authValues.Secret;
			}
			if (!string.IsNullOrEmpty(authValues.AuthParameters))
			{
				dictionary[216] = authValues.AuthParameters;
			}
			if (authValues.AuthPostData != null)
			{
				dictionary[214] = authValues.AuthPostData;
			}
		}
		bool flag = OpCustom(230, dictionary, true, 0, base.IsEncryptionAvailable);
		if (!flag)
		{
			base.Listener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, CustomAuthenticationValues and if you're connected.");
		}
		return flag;
	}

	public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
	{
		if ((int)DebugOut >= 5)
		{
			base.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (groupsToRemove != null)
		{
			dictionary[239] = groupsToRemove;
		}
		if (groupsToAdd != null)
		{
			dictionary[238] = groupsToAdd;
		}
		return OpCustom(248, dictionary, true, 0);
	}

	public virtual bool OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, Hashtable playerProperties, bool onGameServer)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (!string.IsNullOrEmpty(roomName))
		{
			dictionary[byte.MaxValue] = roomName;
		}
		if (lobby != null)
		{
			dictionary[213] = lobby.Name;
			dictionary[212] = (byte)lobby.Type;
		}
		if (onGameServer)
		{
			if (playerProperties != null && playerProperties.Count > 0)
			{
				dictionary[249] = playerProperties;
				dictionary[250] = true;
			}
			if (roomOptions == null)
			{
				roomOptions = new RoomOptions();
			}
			Hashtable hashtable2 = (Hashtable)(dictionary[248] = new Hashtable());
			hashtable2.MergeStringKeys(roomOptions.customRoomProperties);
			hashtable2[(byte)253] = roomOptions.isOpen;
			hashtable2[(byte)254] = roomOptions.isVisible;
			hashtable2[(byte)250] = roomOptions.customRoomPropertiesForLobby;
			if (roomOptions.maxPlayers > 0)
			{
				hashtable2[byte.MaxValue] = roomOptions.maxPlayers;
			}
			if (roomOptions.cleanupCacheOnLeave)
			{
				dictionary[241] = true;
				hashtable2[(byte)249] = true;
			}
		}
		return OpCustom(227, dictionary, true, 0);
	}

	public virtual bool OpFindFriends(string[] friendsToFind)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (friendsToFind != null && friendsToFind.Length != 0)
		{
			dictionary[1] = friendsToFind;
		}
		return OpCustom(222, dictionary, true, 0);
	}

	public virtual bool OpGetRegions(string appId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[224] = appId;
		return OpCustom(220, dictionary, true, 0, true);
	}

	public virtual bool OpJoinLobby(TypedLobby lobby)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
		}
		Dictionary<byte, object> dictionary = null;
		if (lobby != null && !lobby.IsDefault)
		{
			dictionary = new Dictionary<byte, object>();
			dictionary[213] = lobby.Name;
			dictionary[212] = (byte)lobby.Type;
		}
		return OpCustom(229, dictionary, true, 0);
	}

	public virtual bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, Hashtable playerProperties, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
		}
		Hashtable hashtable = new Hashtable();
		hashtable.MergeStringKeys(expectedCustomRoomProperties);
		if (expectedMaxPlayers > 0)
		{
			hashtable[byte.MaxValue] = expectedMaxPlayers;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (hashtable.Count > 0)
		{
			dictionary[248] = hashtable;
		}
		if (playerProperties != null && playerProperties.Count > 0)
		{
			dictionary[249] = playerProperties;
		}
		if (matchingType != 0)
		{
			dictionary[223] = (byte)matchingType;
		}
		if (typedLobby != null)
		{
			dictionary[213] = typedLobby.Name;
			dictionary[212] = (byte)typedLobby.Type;
		}
		if (!string.IsNullOrEmpty(sqlLobbyFilter))
		{
			dictionary[245] = sqlLobbyFilter;
		}
		return OpCustom(225, dictionary, true, 0);
	}

	public virtual bool OpJoinRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, bool createIfNotExists, Hashtable playerProperties, bool onGameServer)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (!string.IsNullOrEmpty(roomName))
		{
			dictionary[byte.MaxValue] = roomName;
		}
		if (createIfNotExists)
		{
			dictionary[215] = true;
			if (lobby != null)
			{
				dictionary[213] = lobby.Name;
				dictionary[212] = (byte)lobby.Type;
			}
		}
		if (onGameServer)
		{
			if (playerProperties != null && playerProperties.Count > 0)
			{
				dictionary[249] = playerProperties;
				dictionary[250] = true;
			}
			if (createIfNotExists)
			{
				if (roomOptions == null)
				{
					roomOptions = new RoomOptions();
				}
				Hashtable hashtable2 = (Hashtable)(dictionary[248] = new Hashtable());
				hashtable2.MergeStringKeys(roomOptions.customRoomProperties);
				hashtable2[(byte)253] = roomOptions.isOpen;
				hashtable2[(byte)254] = roomOptions.isVisible;
				hashtable2[(byte)250] = roomOptions.customRoomPropertiesForLobby;
				if (roomOptions.maxPlayers > 0)
				{
					hashtable2[byte.MaxValue] = roomOptions.maxPlayers;
				}
				if (roomOptions.cleanupCacheOnLeave)
				{
					dictionary[241] = true;
					hashtable2[(byte)249] = true;
				}
			}
		}
		return OpCustom(226, dictionary, true, 0);
	}

	public virtual bool OpLeaveLobby()
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
		}
		return OpCustom(228, null, true, 0);
	}

	public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[244] = eventCode;
		if (customEventContent != null)
		{
			dictionary[245] = customEventContent;
		}
		if (raiseEventOptions == null)
		{
			raiseEventOptions = RaiseEventOptions.Default;
		}
		else
		{
			if (raiseEventOptions.CachingOption != 0)
			{
				dictionary[247] = (byte)raiseEventOptions.CachingOption;
			}
			if (raiseEventOptions.Receivers != 0)
			{
				dictionary[246] = (byte)raiseEventOptions.Receivers;
			}
			if (raiseEventOptions.InterestGroup != 0)
			{
				dictionary[240] = raiseEventOptions.InterestGroup;
			}
			if (raiseEventOptions.TargetActors != null)
			{
				dictionary[252] = raiseEventOptions.TargetActors;
			}
			if (raiseEventOptions.ForwardToWebhook)
			{
				dictionary[234] = true;
			}
		}
		return OpCustom(253, dictionary, sendReliable, raiseEventOptions.SequenceChannel);
	}

	public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
	{
		return OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), broadcast, channelId);
	}

	public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
	{
		return OpSetPropertiesOfRoom(gameProperties.StripToStringKeys(), broadcast, channelId);
	}

	protected bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
		}
		if (actorNr <= 0 || actorProperties == null)
		{
			if ((int)DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
			}
			return false;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(251, actorProperties);
		dictionary.Add(254, actorNr);
		if (broadcast)
		{
			dictionary.Add(250, broadcast);
		}
		return OpCustom(252, dictionary, broadcast, channelId);
	}

	public bool OpSetPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(251, gameProperties);
		if (broadcast)
		{
			dictionary.Add(250, true);
		}
		return OpCustom(252, dictionary, broadcast, channelId);
	}

	protected void OpSetPropertyOfRoom(byte propCode, object value)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[propCode] = value;
		OpSetPropertiesOfRoom(hashtable, true, 0);
	}
}
