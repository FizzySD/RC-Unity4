using Settings;
using UnityEngine;

public class BTN_Server_ASIA : MonoBehaviour
{
	private void OnClick()
	{
		SettingsManager.MultiplayerSettings.ConnectServer(MultiplayerRegion.ASIA);
	}
}
