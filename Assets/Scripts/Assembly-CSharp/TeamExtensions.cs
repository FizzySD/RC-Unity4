using ExitGames.Client.Photon;
using UnityEngine;

internal static class TeamExtensions
{
	public static PunTeams.Team GetTeam(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("team", out value))
		{
			return (PunTeams.Team)(byte)value;
		}
		return PunTeams.Team.none;
	}

	public static void SetTeam(this PhotonPlayer player, PunTeams.Team team)
	{
		if (!PhotonNetwork.connectedAndReady)
		{
			Debug.LogWarning("JoinTeam was called in state: " + PhotonNetwork.connectionStateDetailed.ToString() + ". Not connectedAndReady.");
		}
		if (PhotonNetwork.player.GetTeam() != team)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("team", (byte)team);
			PhotonNetwork.player.SetCustomProperties(hashtable);
		}
	}
}
