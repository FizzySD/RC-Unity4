using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public static class PhotonNetwork
{
	public delegate void EventCallback(byte eventCode, object content, int senderId);

	private static bool _mAutomaticallySyncScene;

	private static bool autoJoinLobbyField;

	public static bool InstantiateInRoomOnly;

	private static bool isOfflineMode;

	internal static int lastUsedViewSubId;

	internal static int lastUsedViewSubIdStatic;

	public static PhotonLogLevel logLevel;

	private static bool m_autoCleanUpPlayerObjects;

	private static bool m_isMessageQueueRunning;

	internal static List<int> manuallyAllocatedViewIds;

	public static readonly int MAX_VIEW_IDS;

	internal static NetworkingPeer networkingPeer;

	private static Room offlineModeRoom;

	public static EventCallback OnEventCall;

	internal static readonly PhotonHandler photonMono;

	public static ServerSettings PhotonServerSettings;

	public static float precisionForFloatSynchronization;

	public static float precisionForQuaternionSynchronization;

	public static float precisionForVectorSynchronization;

	public static Dictionary<string, GameObject> PrefabCache;

	private static int sendInterval;

	private static int sendIntervalOnSerialize;

	public static HashSet<GameObject> SendMonoMessageTargets;

	public const string serverSettingsAssetFile = "PhotonServerSettings";

	public const string serverSettingsAssetPath = "Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset";

	public static bool UseNameServer;

	public static bool UsePrefabCache;

	public const string versionPUN = "1.28";

	public static AuthenticationValues AuthValues
	{
		get
		{
			if (networkingPeer != null)
			{
				return networkingPeer.CustomAuthenticationValues;
			}
			return null;
		}
		set
		{
			if (networkingPeer != null)
			{
				networkingPeer.CustomAuthenticationValues = value;
			}
		}
	}

	public static bool autoCleanUpPlayerObjects
	{
		get
		{
			return m_autoCleanUpPlayerObjects;
		}
		set
		{
			if (room != null)
			{
				Debug.LogError("Setting autoCleanUpPlayerObjects while in a room is not supported.");
			}
			else
			{
				m_autoCleanUpPlayerObjects = value;
			}
		}
	}

	public static bool autoJoinLobby
	{
		get
		{
			return autoJoinLobbyField;
		}
		set
		{
			autoJoinLobbyField = value;
		}
	}

	public static bool automaticallySyncScene
	{
		get
		{
			return _mAutomaticallySyncScene;
		}
		set
		{
			_mAutomaticallySyncScene = value;
			if (_mAutomaticallySyncScene && room != null)
			{
				networkingPeer.LoadLevelIfSynced();
			}
		}
	}

	public static bool connected
	{
		get
		{
			if (offlineMode)
			{
				return true;
			}
			if (networkingPeer == null)
			{
				return false;
			}
			if (!networkingPeer.IsInitialConnect && networkingPeer.State != PeerStates.PeerCreated && networkingPeer.State != PeerStates.Disconnected && networkingPeer.State != PeerStates.Disconnecting)
			{
				return networkingPeer.State != PeerStates.ConnectingToNameServer;
			}
			return false;
		}
	}

	public static bool connectedAndReady
	{
		get
		{
			if (!connected)
			{
				return false;
			}
			if (!offlineMode)
			{
				switch (connectionStateDetailed)
				{
				case PeerStates.PeerCreated:
				case PeerStates.ConnectingToGameserver:
				case PeerStates.Joining:
				case PeerStates.Leaving:
				case PeerStates.ConnectingToMasterserver:
				case PeerStates.Disconnecting:
				case PeerStates.Disconnected:
				case PeerStates.ConnectingToNameServer:
				case PeerStates.Authenticating:
					return false;
				}
			}
			return true;
		}
	}

	public static bool connecting
	{
		get
		{
			if (networkingPeer.IsInitialConnect)
			{
				return !offlineMode;
			}
			return false;
		}
	}

	public static ConnectionState connectionState
	{
		get
		{
			if (offlineMode)
			{
				return ConnectionState.Connected;
			}
			if (networkingPeer != null)
			{
				switch (networkingPeer.PeerState)
				{
				case PeerStateValue.Disconnected:
					return ConnectionState.Disconnected;
				case PeerStateValue.Connecting:
					return ConnectionState.Connecting;
				case PeerStateValue.Connected:
					return ConnectionState.Connected;
				case PeerStateValue.Disconnecting:
					return ConnectionState.Disconnecting;
				case PeerStateValue.InitializingApplication:
					return ConnectionState.InitializingApplication;
				}
			}
			return ConnectionState.Disconnected;
		}
	}

	public static PeerStates connectionStateDetailed
	{
		get
		{
			if (offlineMode)
			{
				if (offlineModeRoom != null)
				{
					return PeerStates.Joined;
				}
				return PeerStates.ConnectedToMaster;
			}
			if (networkingPeer == null)
			{
				return PeerStates.Disconnected;
			}
			return networkingPeer.State;
		}
	}

	public static int countOfPlayers
	{
		get
		{
			return networkingPeer.mPlayersInRoomsCount + networkingPeer.mPlayersOnMasterCount;
		}
	}

	public static int countOfPlayersInRooms
	{
		get
		{
			return networkingPeer.mPlayersInRoomsCount;
		}
	}

	public static int countOfPlayersOnMaster
	{
		get
		{
			return networkingPeer.mPlayersOnMasterCount;
		}
	}

	public static int countOfRooms
	{
		get
		{
			return networkingPeer.mGameCount;
		}
	}

	public static bool CrcCheckEnabled
	{
		get
		{
			return networkingPeer.CrcEnabled;
		}
		set
		{
			if (!connected && !connecting)
			{
				networkingPeer.CrcEnabled = value;
			}
			else
			{
				Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + networkingPeer.CrcEnabled);
			}
		}
	}

	public static List<FriendInfo> Friends { get; set; }

	public static int FriendsListAge
	{
		get
		{
			if (networkingPeer != null)
			{
				return networkingPeer.FriendsListAge;
			}
			return 0;
		}
	}

	public static string gameVersion
	{
		get
		{
			return networkingPeer.mAppVersion;
		}
		set
		{
			networkingPeer.mAppVersion = value;
		}
	}

	public static bool inRoom
	{
		get
		{
			return connectionStateDetailed == PeerStates.Joined;
		}
	}

	public static bool insideLobby
	{
		get
		{
			return networkingPeer.insideLobby;
		}
	}

	public static bool isMasterClient
	{
		get
		{
			if (!offlineMode)
			{
				return networkingPeer.mMasterClient == networkingPeer.mLocalActor;
			}
			return true;
		}
	}

	public static bool isMessageQueueRunning
	{
		get
		{
			return m_isMessageQueueRunning;
		}
		set
		{
			if (value)
			{
				PhotonHandler.StartFallbackSendAckThread();
			}
			networkingPeer.IsSendingOnlyAcks = !value;
			m_isMessageQueueRunning = value;
		}
	}

	public static bool isNonMasterClientInRoom
	{
		get
		{
			if (!isMasterClient)
			{
				return room != null;
			}
			return false;
		}
	}

	public static TypedLobby lobby
	{
		get
		{
			return networkingPeer.lobby;
		}
		set
		{
			networkingPeer.lobby = value;
		}
	}

	public static PhotonPlayer masterClient
	{
		get
		{
			if (networkingPeer == null)
			{
				return null;
			}
			return networkingPeer.mMasterClient;
		}
	}

	[Obsolete("Used for compatibility with Unity networking only.")]
	public static int maxConnections
	{
		get
		{
			if (room == null)
			{
				return 0;
			}
			return room.maxPlayers;
		}
		set
		{
			room.maxPlayers = value;
		}
	}

	public static int MaxResendsBeforeDisconnect
	{
		get
		{
			return networkingPeer.SentCountAllowance;
		}
		set
		{
			if (value < 3)
			{
				value = 3;
			}
			if (value > 10)
			{
				value = 10;
			}
			networkingPeer.SentCountAllowance = value;
		}
	}

	public static bool NetworkStatisticsEnabled
	{
		get
		{
			return networkingPeer.TrafficStatsEnabled;
		}
		set
		{
			networkingPeer.TrafficStatsEnabled = value;
		}
	}

	public static bool offlineMode
	{
		get
		{
			return isOfflineMode;
		}
		set
		{
			if (value == isOfflineMode)
			{
				return;
			}
			if (value && connected)
			{
				Debug.LogError("Can't start OFFLINE mode while connected!");
				return;
			}
			if (networkingPeer.PeerState != 0)
			{
				networkingPeer.Disconnect();
			}
			isOfflineMode = value;
			if (isOfflineMode)
			{
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
				networkingPeer.ChangeLocalID(1);
				networkingPeer.mMasterClient = player;
			}
			else
			{
				offlineModeRoom = null;
				networkingPeer.ChangeLocalID(-1);
				networkingPeer.mMasterClient = null;
			}
		}
	}

	public static PhotonPlayer[] otherPlayers
	{
		get
		{
			if (networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return networkingPeer.mOtherPlayerListCopy;
		}
	}

	public static int PacketLossByCrcCheck
	{
		get
		{
			return networkingPeer.PacketLossByCrc;
		}
	}

	public static PhotonPlayer player
	{
		get
		{
			if (networkingPeer == null)
			{
				return null;
			}
			return networkingPeer.mLocalActor;
		}
	}

	public static PhotonPlayer[] playerList
	{
		get
		{
			if (networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return networkingPeer.mPlayerListCopy;
		}
	}

	public static string playerName
	{
		get
		{
			return networkingPeer.PlayerName;
		}
		set
		{
			networkingPeer.PlayerName = value;
		}
	}

	public static int ResentReliableCommands
	{
		get
		{
			return networkingPeer.ResentReliableCommands;
		}
	}

	public static Room room
	{
		get
		{
			if (isOfflineMode)
			{
				return offlineModeRoom;
			}
			return networkingPeer.mCurrentGame;
		}
	}

	public static int sendRate
	{
		get
		{
			return 1000 / sendInterval;
		}
		set
		{
			sendInterval = 1000 / value;
			if (photonMono != null)
			{
				photonMono.updateInterval = sendInterval;
			}
			if (value < sendRateOnSerialize)
			{
				sendRateOnSerialize = value;
			}
		}
	}

	public static int sendRateOnSerialize
	{
		get
		{
			return 1000 / sendIntervalOnSerialize;
		}
		set
		{
			if (value > sendRate)
			{
				Debug.LogError("Error, can not set the OnSerialize SendRate more often then the overall SendRate");
				value = sendRate;
			}
			sendIntervalOnSerialize = 1000 / value;
			if (photonMono != null)
			{
				photonMono.updateIntervalOnSerialize = sendIntervalOnSerialize;
			}
		}
	}

	public static ServerConnection Server
	{
		get
		{
			return networkingPeer.server;
		}
	}

	public static string ServerAddress
	{
		get
		{
			if (networkingPeer != null)
			{
				return networkingPeer.ServerAddress;
			}
			return "<not connected>";
		}
	}

	public static double time
	{
		get
		{
			if (offlineMode)
			{
				return Time.time;
			}
			return (double)networkingPeer.ServerTimeInMilliSeconds / 1000.0;
		}
	}

	public static int unreliableCommandsLimit
	{
		get
		{
			return networkingPeer.LimitOfUnreliableCommands;
		}
		set
		{
			networkingPeer.LimitOfUnreliableCommands = value;
		}
	}

	static PhotonNetwork()
	{
		_mAutomaticallySyncScene = false;
		autoJoinLobbyField = true;
		InstantiateInRoomOnly = true;
		isOfflineMode = false;
		lastUsedViewSubId = 0;
		lastUsedViewSubIdStatic = 0;
		logLevel = PhotonLogLevel.ErrorsOnly;
		m_autoCleanUpPlayerObjects = true;
		m_isMessageQueueRunning = true;
		manuallyAllocatedViewIds = new List<int>();
		MAX_VIEW_IDS = 1000;
		offlineModeRoom = null;
		PhotonServerSettings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));
		precisionForFloatSynchronization = 0.01f;
		precisionForQuaternionSynchronization = 1f;
		precisionForVectorSynchronization = 9.9E-05f;
		PrefabCache = new Dictionary<string, GameObject>();
		sendInterval = 50;
		sendIntervalOnSerialize = 100;
		UseNameServer = true;
		UsePrefabCache = true;
		Application.runInBackground = true;
		GameObject gameObject = new GameObject();
		photonMono = gameObject.AddComponent<PhotonHandler>();
		gameObject.name = "PhotonMono";
		gameObject.hideFlags = HideFlags.HideInHierarchy;
		networkingPeer = new NetworkingPeer(photonMono, string.Empty, ConnectionProtocol.Udp);
		CustomTypes.Register();
	}

	private static int[] AllocateSceneViewIDs(int countOfNewViews)
	{
		int[] array = new int[countOfNewViews];
		for (int i = 0; i < countOfNewViews; i++)
		{
			array[i] = AllocateViewID(0);
		}
		return array;
	}

	public static int AllocateViewID()
	{
		int num = AllocateViewID(player.ID);
		manuallyAllocatedViewIds.Add(num);
		return num;
	}

	private static int AllocateViewID(int ownerId)
	{
		if (ownerId == 0)
		{
			int num = lastUsedViewSubIdStatic;
			int num2 = ownerId * MAX_VIEW_IDS;
			for (int i = 1; i < MAX_VIEW_IDS; i++)
			{
				num = (num + 1) % MAX_VIEW_IDS;
				if (num != 0)
				{
					int num3 = num + num2;
					if (!networkingPeer.photonViewList.ContainsKey(num3))
					{
						lastUsedViewSubIdStatic = num;
						return num3;
					}
				}
			}
			throw new Exception(string.Format("AllocateViewID() failed. Room (user {0}) is out of subIds, as all room viewIDs are used.", ownerId));
		}
		int num4 = lastUsedViewSubId;
		int num5 = ownerId * MAX_VIEW_IDS;
		for (int j = 1; j < MAX_VIEW_IDS; j++)
		{
			num4 = (num4 + 1) % MAX_VIEW_IDS;
			if (num4 != 0)
			{
				int num6 = num4 + num5;
				if (!networkingPeer.photonViewList.ContainsKey(num6) && !manuallyAllocatedViewIds.Contains(num6))
				{
					lastUsedViewSubId = num4;
					return num6;
				}
			}
		}
		throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of subIds, as all viewIDs are used.", ownerId));
	}

	public static bool CloseConnection(PhotonPlayer kickPlayer)
	{
		if (!VerifyCanUseNetwork())
		{
			return false;
		}
		if (!player.isMasterClient)
		{
			Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
			return false;
		}
		if (kickPlayer == null)
		{
			Debug.LogError("CloseConnection: No such player connected!");
			return false;
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.TargetActors = new int[1] { kickPlayer.ID };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		return networkingPeer.OpRaiseEvent(203, null, true, raiseEventOptions2);
	}

	public static bool ConnectToBestCloudServer(string gameVersion)
	{
		if (PhotonServerSettings == null)
		{
			Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			return ConnectUsingSettings(gameVersion);
		}
		networkingPeer.IsInitialConnect = true;
		networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
		CloudRegionCode bestRegionCodeInPreferences = PhotonHandler.BestRegionCodeInPreferences;
		if (bestRegionCodeInPreferences != CloudRegionCode.none)
		{
			Debug.Log("Best region found in PlayerPrefs. Connecting to: " + bestRegionCodeInPreferences);
			return networkingPeer.ConnectToRegionMaster(bestRegionCodeInPreferences);
		}
		return networkingPeer.ConnectToNameServer();
	}

	public static bool ConnectToMaster(string masterServerAddress, int port, string appID, string gameVersion)
	{
		if (networkingPeer.PeerState != 0)
		{
			Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
			return false;
		}
		if (offlineMode)
		{
			offlineMode = false;
			Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
		}
		if (!isMessageQueueRunning)
		{
			isMessageQueueRunning = true;
			Debug.LogWarning("ConnectToMaster() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		networkingPeer.SetApp(appID, gameVersion);
		networkingPeer.IsUsingNameServer = false;
		networkingPeer.IsInitialConnect = true;
		networkingPeer.MasterServerAddress = masterServerAddress + ":" + port;
		return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
	}

	public static bool ConnectUsingSettings(string gameVersion)
	{
		if (PhotonServerSettings == null)
		{
			Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		SwitchToProtocol(PhotonServerSettings.Protocol);
		networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			offlineMode = true;
			return true;
		}
		if (offlineMode)
		{
			Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
		}
		offlineMode = false;
		isMessageQueueRunning = true;
		networkingPeer.IsInitialConnect = true;
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted)
		{
			networkingPeer.IsUsingNameServer = false;
			networkingPeer.MasterServerAddress = PhotonServerSettings.ServerAddress + ":" + PhotonServerSettings.ServerPort;
			return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
		}
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
		{
			return ConnectToBestCloudServer(gameVersion);
		}
		return networkingPeer.ConnectToRegionMaster(PhotonServerSettings.PreferredRegion);
	}

	public static bool CreateRoom(string roomName)
	{
		return CreateRoom(roomName, null, null);
	}

	public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		if (offlineMode)
		{
			if (offlineModeRoom != null)
			{
				Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			offlineModeRoom = new Room(roomName, roomOptions);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return true;
		}
		if (networkingPeer.server == ServerConnection.MasterServer && connectedAndReady)
		{
			return networkingPeer.OpCreateGame(roomName, roomOptions, typedLobby);
		}
		Debug.LogError("CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
		return false;
	}

	[Obsolete("Use overload with RoomOptions and TypedLobby parameters.")]
	public static bool CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers)
	{
		RoomOptions roomOptions = new RoomOptions
		{
			isVisible = isVisible,
			isOpen = isOpen,
			maxPlayers = maxPlayers
		};
		return CreateRoom(roomName, roomOptions, null);
	}

	[Obsolete("Use overload with RoomOptions and TypedLobby parameters.")]
	public static bool CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers, Hashtable customRoomProperties, string[] propsToListInLobby)
	{
		RoomOptions roomOptions = new RoomOptions
		{
			isVisible = isVisible,
			isOpen = isOpen,
			maxPlayers = maxPlayers,
			customRoomProperties = customRoomProperties,
			customRoomPropertiesForLobby = propsToListInLobby
		};
		return CreateRoom(roomName, roomOptions, null);
	}

	public static void Destroy(PhotonView targetView)
	{
		if (targetView != null)
		{
			networkingPeer.RemoveInstantiatedGO(targetView.gameObject, !inRoom);
		}
		else
		{
			Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
		}
	}

	public static void Destroy(GameObject targetGo)
	{
		networkingPeer.RemoveInstantiatedGO(targetGo, !inRoom);
	}

	public static void DestroyAll()
	{
		if (isMasterClient)
		{
			networkingPeer.DestroyAll(false);
		}
		else
		{
			Debug.LogError("Couldn't call DestroyAll() as only the master client is allowed to call this.");
		}
	}

	public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
	{
		if (player == null)
		{
			Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
		}
		DestroyPlayerObjects(targetPlayer.ID);
	}

	public static void DestroyPlayerObjects(int targetPlayerId)
	{
		if (VerifyCanUseNetwork())
		{
			if (player.isMasterClient || targetPlayerId == player.ID)
			{
				networkingPeer.DestroyPlayerObjects(targetPlayerId, false);
			}
			else
			{
				Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + isMasterClient);
			}
		}
	}

	public static void Disconnect()
	{
		if (offlineMode)
		{
			offlineMode = false;
			offlineModeRoom = null;
			networkingPeer.State = PeerStates.Disconnecting;
			networkingPeer.OnStatusChanged(StatusCode.Disconnect);
		}
		else if (networkingPeer != null)
		{
			networkingPeer.Disconnect();
		}
	}

	public static void FetchServerTimestamp()
	{
		if (networkingPeer != null)
		{
			networkingPeer.FetchServerTimestamp();
		}
	}

	public static bool FindFriends(string[] friendsToFind)
	{
		if (networkingPeer != null && !isOfflineMode)
		{
			return networkingPeer.OpFindFriends(friendsToFind);
		}
		return false;
	}

	public static int GetPing()
	{
		return networkingPeer.RoundTripTime;
	}

	public static RoomInfo[] GetRoomList()
	{
		if (!offlineMode && networkingPeer != null)
		{
			return networkingPeer.mGameListCopy;
		}
		return new RoomInfo[0];
	}

	[Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
	public static void InitializeSecurity()
	{
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group)
	{
		return Instantiate(prefabName, position, rotation, group, null);
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		if (!connected || (InstantiateInRoomOnly && !inRoom))
		{
			Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + connectionStateDetailed);
			return null;
		}
		GameObject value;
		if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out value))
		{
			value = ((!prefabName.StartsWith("RCAsset/")) ? ((GameObject)Resources.Load(prefabName, typeof(GameObject))) : FengGameManagerMKII.InstantiateCustomAsset(prefabName));
			if (UsePrefabCache)
			{
				PrefabCache.Add(prefabName, value);
			}
		}
		if (value == null)
		{
			Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (value.GetComponent<PhotonView>() == null)
		{
			Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
			return null;
		}
		int[] array = new int[value.GetPhotonViewsInChildren().Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = AllocateViewID(player.ID);
		}
		Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, false);
		return networkingPeer.DoInstantiate2(evData, networkingPeer.mLocalActor, value);
	}

	public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		if (!connected || (InstantiateInRoomOnly && !inRoom))
		{
			Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + connectionStateDetailed);
			return null;
		}
		if (!isMasterClient)
		{
			Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client is not the MasterClient in this room.");
			return null;
		}
		GameObject value;
		if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out value))
		{
			value = (GameObject)Resources.Load(prefabName, typeof(GameObject));
			if (UsePrefabCache)
			{
				PrefabCache.Add(prefabName, value);
			}
		}
		if (value == null)
		{
			Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (value.GetComponent<PhotonView>() == null)
		{
			Debug.LogError("Failed to InstantiateSceneObject prefab:" + prefabName + ". Prefab must have a PhotonView component.");
			return null;
		}
		int[] array = AllocateSceneViewIDs(value.GetPhotonViewsInChildren().Length);
		if (array == null)
		{
			Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". No ViewIDs are free to use. Max is: " + MAX_VIEW_IDS);
			return null;
		}
		Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, true);
		return networkingPeer.DoInstantiate2(evData, networkingPeer.mLocalActor, value);
	}

	public static void InternalCleanPhotonMonoFromSceneIfStuck()
	{
		PhotonHandler[] array = UnityEngine.Object.FindObjectsOfType(typeof(PhotonHandler)) as PhotonHandler[];
		if (array == null || array.Length == 0)
		{
			return;
		}
		Debug.Log("Cleaning up hidden PhotonHandler instances in scene. Please save it. This is not an issue.");
		PhotonHandler[] array2 = array;
		foreach (PhotonHandler photonHandler in array2)
		{
			photonHandler.gameObject.hideFlags = HideFlags.None;
			if (photonHandler.gameObject != null && photonHandler.gameObject.name == "PhotonMono")
			{
				UnityEngine.Object.DestroyImmediate(photonHandler.gameObject);
			}
			UnityEngine.Object.DestroyImmediate(photonHandler);
		}
	}

	public static bool JoinLobby()
	{
		return JoinLobby(null);
	}

	public static bool JoinLobby(TypedLobby typedLobby)
	{
		if (!connected || Server != 0)
		{
			return false;
		}
		if (typedLobby == null)
		{
			typedLobby = TypedLobby.Default;
		}
		bool flag = networkingPeer.OpJoinLobby(typedLobby);
		if (flag)
		{
			networkingPeer.lobby = typedLobby;
		}
		return flag;
	}

	public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		if (offlineMode)
		{
			if (offlineModeRoom != null)
			{
				Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			offlineModeRoom = new Room(roomName, roomOptions);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return true;
		}
		if (networkingPeer.server != 0 || !connectedAndReady)
		{
			Debug.LogError("JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		if (string.IsNullOrEmpty(roomName))
		{
			Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
			return false;
		}
		return networkingPeer.OpJoinRoom(roomName, roomOptions, typedLobby, true);
	}

	public static bool JoinRandomRoom()
	{
		return JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null);
	}

	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
	{
		return JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null);
	}

	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
	{
		if (offlineMode)
		{
			if (offlineModeRoom != null)
			{
				Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			offlineModeRoom = new Room("offline room", null);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return true;
		}
		if (networkingPeer.server != 0 || !connectedAndReady)
		{
			Debug.LogError("JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.MergeStringKeys(expectedCustomRoomProperties);
		if (expectedMaxPlayers > 0)
		{
			hashtable[byte.MaxValue] = expectedMaxPlayers;
		}
		return networkingPeer.OpJoinRandomRoom(hashtable, 0, null, matchingType, typedLobby, sqlLobbyFilter);
	}

	public static bool JoinRoom(string roomName)
	{
		if (offlineMode)
		{
			if (offlineModeRoom != null)
			{
				Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			offlineModeRoom = new Room(roomName, null);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return true;
		}
		if (networkingPeer.server != 0 || !connectedAndReady)
		{
			Debug.LogError("JoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		if (string.IsNullOrEmpty(roomName))
		{
			Debug.LogError("JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
			return false;
		}
		return networkingPeer.OpJoinRoom(roomName, null, null, false);
	}

	[Obsolete("Use overload with roomOptions and TypedLobby parameter.")]
	public static bool JoinRoom(string roomName, bool createIfNotExists)
	{
		if (connectionStateDetailed == PeerStates.Joining || connectionStateDetailed == PeerStates.Joined || connectionStateDetailed == PeerStates.ConnectedToGameserver)
		{
			Debug.LogError("JoinRoom aborted: You can only join a room while not currently connected/connecting to a room.");
		}
		else if (room != null)
		{
			Debug.LogError("JoinRoom aborted: You are already in a room!");
		}
		else
		{
			if (!(roomName == string.Empty))
			{
				if (offlineMode)
				{
					offlineModeRoom = new Room(roomName, null);
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
					return true;
				}
				return networkingPeer.OpJoinRoom(roomName, null, null, createIfNotExists);
			}
			Debug.LogError("JoinRoom aborted: You must specifiy a room name!");
		}
		return false;
	}

	public static bool LeaveLobby()
	{
		if (connected && Server == ServerConnection.MasterServer)
		{
			return networkingPeer.OpLeaveLobby();
		}
		return false;
	}

	public static bool RejoinRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, bool createIfNotExists, Hashtable Hash)
	{
		bool flag = networkingPeer.server == ServerConnection.GameServer;
		if (!flag)
		{
			Room room = new Room(roomName, roomOptions);
			networkingPeer.mRoomToEnterLobby = null;
			if (createIfNotExists)
			{
				networkingPeer.mRoomToEnterLobby = ((!networkingPeer.insideLobby) ? null : networkingPeer.lobby);
			}
		}
		return networkingPeer.OpJoinRoom(roomName, roomOptions, networkingPeer.mRoomToEnterLobby, createIfNotExists, Hash, flag);
	}

	public static bool LeaveRoom()
	{
		if (offlineMode)
		{
			offlineModeRoom = null;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
			return true;
		}
		if (room == null)
		{
			Debug.LogWarning("PhotonNetwork.room is null. You don't have to call LeaveRoom() when you're not in one. State: " + connectionStateDetailed);
		}
		return networkingPeer.OpLeave();
	}

	public static void LoadLevel(int levelNumber)
	{
		networkingPeer.SetLevelInPropsIfSynced(levelNumber);
		isMessageQueueRunning = false;
		networkingPeer.loadingLevelAndPausedNetwork = true;
		Application.LoadLevel(levelNumber);
	}

	public static void LoadLevel(string levelName)
	{
		networkingPeer.SetLevelInPropsIfSynced(levelName);
		isMessageQueueRunning = false;
		networkingPeer.loadingLevelAndPausedNetwork = true;
		Application.LoadLevel(levelName);
	}

	public static void NetworkStatisticsReset()
	{
		networkingPeer.TrafficStatsReset();
	}

	public static string NetworkStatisticsToString()
	{
		if (networkingPeer != null && !offlineMode)
		{
			return networkingPeer.VitalStatsToString(false);
		}
		return "Offline or in OfflineMode. No VitalStats available.";
	}

	public static void OverrideBestCloudServer(CloudRegionCode region)
	{
		PhotonHandler.BestRegionCodeInPreferences = region;
	}

	public static bool RaiseEvent(byte eventCode, object eventContent, bool sendReliable, RaiseEventOptions options)
	{
		if (inRoom && eventCode < byte.MaxValue)
		{
			return networkingPeer.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
		}
		Debug.LogWarning("RaiseEvent() failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
		return false;
	}

	public static void RefreshCloudServerRating()
	{
		throw new NotImplementedException("not available at the moment");
	}

	public static void RemoveRPCs(PhotonPlayer targetPlayer)
	{
		if (VerifyCanUseNetwork())
		{
			if (!targetPlayer.isLocal && !isMasterClient)
			{
				Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
			}
			else
			{
				networkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
			}
		}
	}

	public static void RemoveRPCs(PhotonView targetPhotonView)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.CleanRpcBufferIfMine(targetPhotonView);
		}
	}

	public static void RemoveRPCsInGroup(int targetGroup)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.RemoveRPCsInGroup(targetGroup);
		}
	}

	internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		if (VerifyCanUseNetwork())
		{
			if (room == null)
			{
				Debug.LogWarning("Cannot send RPCs in Lobby, only processed locally");
			}
			else
			{
				if (player == null)
				{
					Debug.LogError("Error; Sending RPC to player null! Aborted \"" + methodName + "\"");
				}
				if (networkingPeer != null)
				{
					networkingPeer.RPC(view, methodName, targetPlayer, parameters);
				}
				else
				{
					Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
				}
			}
		}
		LogRPC(methodName, parameters);
	}

	internal static void RPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
	{
		if (VerifyCanUseNetwork())
		{
			if (room == null)
			{
				Debug.LogWarning("Cannot send RPCs in Lobby! RPC dropped.");
			}
			else if (networkingPeer != null)
			{
				networkingPeer.RPC(view, methodName, target, parameters);
			}
			else
			{
				Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
			}
		}
		LogRPC(methodName, parameters);
	}

	private static void LogRPC(string methodName, params object[] parameters)
	{
	}

	public static void SendOutgoingCommands()
	{
		if (VerifyCanUseNetwork())
		{
			while (networkingPeer.SendOutgoingCommands())
			{
			}
		}
	}

	public static void SetLevelPrefix(short prefix)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetLevelPrefix(prefix);
		}
	}

	public static bool SetMasterClient(PhotonPlayer masterClientPlayer)
	{
		return networkingPeer.SetMasterClient(masterClientPlayer.ID, true);
	}

	public static void SetPlayerCustomProperties(Hashtable customProperties)
	{
		if (customProperties == null)
		{
			customProperties = new Hashtable();
			foreach (object key in player.customProperties.Keys)
			{
				customProperties[(string)key] = null;
			}
		}
		if (room != null && room.isLocalClientInside)
		{
			player.SetCustomProperties(customProperties);
		}
		else
		{
			player.InternalCacheProperties(customProperties);
		}
	}

	public static void SetReceivingEnabled(int group, bool enabled)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetReceivingEnabled(group, enabled);
		}
	}

	public static void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetReceivingEnabled(enableGroups, disableGroups);
		}
	}

	public static void SetSendingEnabled(int group, bool enabled)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetSendingEnabled(group, enabled);
		}
	}

	public static void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetSendingEnabled(enableGroups, disableGroups);
		}
	}

	public static void SwitchToProtocol(ConnectionProtocol cp)
	{
		if (networkingPeer.UsedProtocol != cp)
		{
			try
			{
				networkingPeer.Disconnect();
				networkingPeer.StopThread();
			}
			catch
			{
			}
			networkingPeer = new NetworkingPeer(photonMono, string.Empty, cp);
			Debug.Log("Protocol switched to: " + cp);
		}
	}

	public static void UnAllocateViewID(int viewID)
	{
		manuallyAllocatedViewIds.Remove(viewID);
		if (networkingPeer.photonViewList.ContainsKey(viewID))
		{
			Debug.LogWarning(string.Format("Unallocated manually used viewID: {0} but found it used still in a PhotonView: {1}", viewID, networkingPeer.photonViewList[viewID]));
		}
	}

	private static bool VerifyCanUseNetwork()
	{
		if (connected)
		{
			return true;
		}
		Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
		return false;
	}

	public static bool WebRpc(string name, object parameters)
	{
		return networkingPeer.WebRpc(name, parameters);
	}
}
