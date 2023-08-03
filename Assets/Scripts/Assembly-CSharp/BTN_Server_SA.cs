using Settings;
using UnityEngine;

public class BTN_Server_SA : MonoBehaviour
{
	private void OnClick()
	{
		SettingsManager.MultiplayerSettings.ConnectServer(MultiplayerRegion.SA);
	}
}
