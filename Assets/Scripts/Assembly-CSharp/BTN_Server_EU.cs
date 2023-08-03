using Settings;
using UnityEngine;

public class BTN_Server_EU : MonoBehaviour
{
	private void OnClick()
	{
		SettingsManager.MultiplayerSettings.ConnectServer(MultiplayerRegion.EU);
	}
}
