using Settings;
using UnityEngine;

public class BTN_ServerUS : MonoBehaviour
{
	private void OnClick()
	{
		SettingsManager.MultiplayerSettings.ConnectServer(MultiplayerRegion.US);
	}
}
