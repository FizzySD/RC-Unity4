using Settings;
using UnityEngine;

public class BombUtil
{
	public static Color GetBombColor(PhotonPlayer player, float minAlpha = 0.5f)
	{
		if (SettingsManager.LegacyGameSettings.TeamMode.Value > 0)
		{
			switch (RCextensions.returnIntFromObject(player.customProperties[PhotonPlayerProperty.RCteam]))
			{
			case 1:
				return Color.cyan;
			case 2:
				return Color.magenta;
			}
		}
		return GetBombColorIndividual(player, minAlpha);
	}

	private static Color GetBombColorIndividual(PhotonPlayer player, float minAlpha)
	{
		float r = RCextensions.returnFloatFromObject(player.customProperties[PhotonPlayerProperty.RCBombR]);
		float g = RCextensions.returnFloatFromObject(player.customProperties[PhotonPlayerProperty.RCBombG]);
		float b = RCextensions.returnFloatFromObject(player.customProperties[PhotonPlayerProperty.RCBombB]);
		float b2 = RCextensions.returnFloatFromObject(player.customProperties[PhotonPlayerProperty.RCBombA]);
		b2 = Mathf.Max(minAlpha, b2);
		return new Color(r, g, b, b2);
	}
}
