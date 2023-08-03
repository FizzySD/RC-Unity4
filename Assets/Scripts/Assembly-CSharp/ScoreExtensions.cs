using ExitGames.Client.Photon;

internal static class ScoreExtensions
{
	public static void AddScore(this PhotonPlayer player, int scoreToAddToCurrent)
	{
		int num = player.GetScore() + scoreToAddToCurrent;
		Hashtable hashtable = new Hashtable();
		hashtable["score"] = num;
		player.SetCustomProperties(hashtable);
	}

	public static int GetScore(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("score", out value))
		{
			return (int)value;
		}
		return 0;
	}

	public static void SetScore(this PhotonPlayer player, int newScore)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["score"] = newScore;
		player.SetCustomProperties(hashtable);
	}
}
