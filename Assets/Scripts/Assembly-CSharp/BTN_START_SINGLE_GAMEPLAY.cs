using UnityEngine;

public class BTN_START_SINGLE_GAMEPLAY : MonoBehaviour
{
	private void OnClick()
	{
		string selection = GameObject.Find("PopupListMap").GetComponent<UIPopupList>().selection;
		string selection2 = GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection;
		int difficulty = (GameObject.Find("CheckboxHard").GetComponent<UICheckbox>().isChecked ? 1 : (GameObject.Find("CheckboxAbnormal").GetComponent<UICheckbox>().isChecked ? 2 : 0));
		IN_GAME_MAIN_CAMERA.difficulty = difficulty;
		IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.SINGLE;
		IN_GAME_MAIN_CAMERA.singleCharacter = selection2.ToUpper();
		if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
		{
			Screen.lockCursor = true;
		}
		Screen.showCursor = false;
		if (selection == "trainning_0")
		{
			IN_GAME_MAIN_CAMERA.difficulty = -1;
		}
		FengGameManagerMKII.level = selection;
		Application.LoadLevel(LevelInfo.getInfo(selection).mapName);
	}
}
