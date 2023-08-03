using UnityEngine;

public class PopListCamera : MonoBehaviour
{
	private void Awake()
	{
		if (PlayerPrefs.HasKey("cameraType"))
		{
			GetComponent<UIPopupList>().selection = PlayerPrefs.GetString("cameraType");
		}
	}

	private void OnSelectionChange()
	{
		if (GetComponent<UIPopupList>().selection == "ORIGINAL")
		{
			IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.ORIGINAL;
		}
		if (GetComponent<UIPopupList>().selection == "WOW")
		{
			IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.WOW;
		}
		if (GetComponent<UIPopupList>().selection == "TPS")
		{
			IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.TPS;
		}
		PlayerPrefs.SetString("cameraType", GetComponent<UIPopupList>().selection);
	}
}
