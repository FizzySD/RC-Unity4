using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
	internal class MultiplayerSettings : SaveableSettingsContainer
	{
		public static string PublicLobby = "01042015";

		public static string PrivateLobby = "verified343";

		public static string PublicAppId = "e3341a7b-1cb9-4b6c-90e4-39b796e52876";

		public IntSetting LobbyMode = new IntSetting(0);

		public IntSetting AppIdMode = new IntSetting(0);

		public StringSetting CustomLobby = new StringSetting(string.Empty);

		public StringSetting CustomAppId = new StringSetting(string.Empty);

		public StringSetting LanIP = new StringSetting(string.Empty);

		public IntSetting LanPort = new IntSetting(5055);

		public StringSetting LanPassword = new StringSetting(string.Empty);

		public MultiplayerServerType CurrentMultiplayerServerType;

		public readonly Dictionary<MultiplayerRegion, string> CloudAddresses = new Dictionary<MultiplayerRegion, string>
		{
			{
				MultiplayerRegion.EU,
				"app-eu.exitgames.com"
			},
			{
				MultiplayerRegion.US,
				"app-us.exitgames.com"
			},
			{
				MultiplayerRegion.SA,
				"app-sa.exitgames.com"
			},
			{
				MultiplayerRegion.ASIA,
				"app-asia.exitgames.com"
			}
		};

		public readonly Dictionary<MultiplayerRegion, string> PublicAddresses = new Dictionary<MultiplayerRegion, string>
		{
			{
				MultiplayerRegion.EU,
                "app-eu.exitgames.com"
            },
			{
				MultiplayerRegion.US,
                "app-us.exitgames.com"
            },
			{
				MultiplayerRegion.SA,
                "app-sa.exitgames.com"
            },
			{
				MultiplayerRegion.ASIA,
                "app-asia.exitgames.com"
            }
		};

		public readonly int DefaultPort = 5055;

		public StringSetting Name = new StringSetting("GUEST" + Random.Range(0, 100000), 50);

		public StringSetting Guild = new StringSetting(string.Empty, 50);

		protected override string FileName
		{
			get
			{
				return "Multiplayer.json";
			}
		}

		public void ConnectServer(MultiplayerRegion region)
		{
			FengGameManagerMKII.JustLeftRoom = false;
			PhotonNetwork.Disconnect();
			if (AppIdMode.Value == 0)
			{
				string masterServerAddress = PublicAddresses[region];
				CurrentMultiplayerServerType = MultiplayerServerType.Cloud;
				PhotonNetwork.ConnectToMaster(masterServerAddress, DefaultPort, PublicAppId, GetCurrentLobby());
			}
			else
			{
				string masterServerAddress = PublicAddresses[region];
				CurrentMultiplayerServerType = MultiplayerServerType.Cloud;
				PhotonNetwork.ConnectToMaster(masterServerAddress, DefaultPort, CustomAppId.Value, GetCurrentLobby());
			}
		}

		public string GetCurrentLobby()
		{
			if (LobbyMode.Value == 0)
			{
				return PublicLobby;
			}
			if (LobbyMode.Value == 1)
			{
				return PrivateLobby;
			}
			return CustomLobby.Value;
		}

		public void ConnectLAN()
		{
			PhotonNetwork.Disconnect();
			if (PhotonNetwork.ConnectToMaster(LanIP.Value, LanPort.Value, string.Empty, GetCurrentLobby()))
			{
				CurrentMultiplayerServerType = MultiplayerServerType.LAN;
				FengGameManagerMKII.PrivateServerAuthPass = LanPassword.Value;
			}
		}

		public void ConnectOffline()
		{
			PhotonNetwork.Disconnect();
			PhotonNetwork.offlineMode = true;
			CurrentMultiplayerServerType = MultiplayerServerType.Cloud;
			FengGameManagerMKII.instance.OnJoinedLobby();
		}
	}
}
