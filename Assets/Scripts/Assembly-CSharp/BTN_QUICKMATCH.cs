using Settings;
using UnityEngine;

public class BTN_QUICKMATCH : MonoBehaviour
{
	private void OnClick()
	{
		SettingsManager.MultiplayerSettings.ConnectOffline();
	}
}
