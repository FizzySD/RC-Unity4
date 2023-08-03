using UnityEngine;

public class BTN_START_MULTI_SERVER : MonoBehaviour
{
	private void OnClick()
	{
		string text = GameObject.Find("InputServerName").GetComponent<UIInput>().label.text;
		int maxPlayers = int.Parse(GameObject.Find("InputMaxPlayer").GetComponent<UIInput>().label.text);
		int num = int.Parse(GameObject.Find("InputMaxTime").GetComponent<UIInput>().label.text);
		string selection = GameObject.Find("PopupListMap").GetComponent<UIPopupList>().selection;
		string text2 = (GameObject.Find("CheckboxHard").GetComponent<UICheckbox>().isChecked ? "hard" : ((!GameObject.Find("CheckboxAbnormal").GetComponent<UICheckbox>().isChecked) ? "normal" : "abnormal"));
		string text3 = "day";
		string text4 = GameObject.Find("InputStartServerPWD").GetComponent<UIInput>().label.text;
		if (text4.Length > 0)
		{
			text4 = new SimpleAES().Encrypt(text4);
		}
		PhotonNetwork.CreateRoom(text + "`" + selection + "`" + text2 + "`" + num + "`" + text3 + "`" + text4 + "`" + Random.Range(0, 50000), true, true, maxPlayers);
	}
}
