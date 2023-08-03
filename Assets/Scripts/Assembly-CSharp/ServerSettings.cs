using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

[Serializable]
public class ServerSettings : ScriptableObject
{
	public enum HostingOption
	{
		NotSet = 0,
		PhotonCloud = 1,
		SelfHosted = 2,
		OfflineMode = 3,
		BestRegion = 4
	}

	public string AppID = string.Empty;

	[HideInInspector]
	public bool DisableAutoOpenWizard;

	public HostingOption HostType;

	public bool PingCloudServersOnAwake;

	public CloudRegionCode PreferredRegion;

	public ConnectionProtocol Protocol;

	public List<string> RpcList = new List<string>();

	public string ServerAddress = string.Empty;

	public int ServerPort = 5055;

	public override string ToString()
	{
		object[] array = new object[4] { "ServerSettings: ", HostType, " ", ServerAddress };
		return string.Concat(array);
	}

	public void UseCloud(string cloudAppid)
	{
		HostType = HostingOption.PhotonCloud;
		AppID = cloudAppid;
	}

	public void UseCloud(string cloudAppid, CloudRegionCode code)
	{
		HostType = HostingOption.PhotonCloud;
		AppID = cloudAppid;
		PreferredRegion = code;
	}

	public void UseCloudBestResion(string cloudAppid)
	{
		HostType = HostingOption.BestRegion;
		AppID = cloudAppid;
	}

	public void UseMyServer(string serverAddress, int serverPort, string application)
	{
		HostType = HostingOption.SelfHosted;
		AppID = ((application == null) ? "master" : application);
		ServerAddress = serverAddress;
		ServerPort = serverPort;
	}
}
