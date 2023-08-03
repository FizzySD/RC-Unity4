using UnityEngine;

public class BTN_ToJoin : MonoBehaviour
{
	public static GameObject CreateInput(GameObject parent, GameObject toClone, Vector3 position, Quaternion rotation, string name, string hint, uint width = 100u, int maxChars = 100, bool isPassword = false)
	{
		GameObject prefab = (GameObject)Object.Instantiate(toClone);
		GameObject gameObject = NGUITools.AddChild(parent, prefab);
		gameObject.name = name;
		gameObject.transform.localPosition = position;
		gameObject.transform.localRotation = rotation;
		gameObject.transform.Find("Label").gameObject.GetComponent<UILabel>().text = hint;
		gameObject.GetComponent<UIInput>().isPassword = isPassword;
		gameObject.GetComponent<UIInput>().maxChars = maxChars;
		Vector3 size = gameObject.GetComponent<BoxCollider>().size;
		float x = size.x;
		size.x = width;
		gameObject.GetComponent<BoxCollider>().size = size;
		gameObject.GetComponent<UIInput>().label.lineWidth = (int)width;
		size = gameObject.transform.Find("Background").localScale;
		size.x *= (float)width / x;
		gameObject.transform.Find("Background").localScale = size;
		gameObject.transform.Find("Background").position = gameObject.GetComponent<UIInput>().label.transform.position;
		return gameObject;
	}

	public static GameObject CreateLabel(GameObject parent, GameObject toClone, Vector3 position, Quaternion rotation, string name, string text, int fontsize, int lineWidth = 130)
	{
		GameObject prefab = (GameObject)Object.Instantiate(toClone);
		GameObject gameObject = NGUITools.AddChild(parent, prefab);
		gameObject.name = name;
		gameObject.transform.localPosition = position;
		gameObject.transform.localRotation = rotation;
		gameObject.GetComponent<UILabel>().text = text;
		gameObject.GetComponent<UILabel>().font.dynamicFontSize = fontsize;
		gameObject.GetComponent<UILabel>().lineWidth = lineWidth;
		return gameObject;
	}

	private void OnClick()
	{
		NGUITools.SetActive(base.transform.parent.gameObject, false);
		NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiJoinPrivate, true);
		Transform transform = GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiJoinPrivate.transform;
		Transform transform2 = transform.Find("ButtonJOIN");
		if (transform2.GetComponent<BTN_Join_LAN>() == null)
		{
			transform2.gameObject.AddComponent<BTN_Join_LAN>();
		}
		Transform transform3 = transform.Find("InputIP");
		Transform transform4 = transform.Find("InputPort");
		string @string = PlayerPrefs.GetString("lastIP", "127.0.0.1");
		string string2 = PlayerPrefs.GetString("lastPort", "5055");
		transform3.GetComponent<UIInput>().text = @string;
		transform3.GetComponent<UIInput>().label.text = @string;
		transform4.GetComponent<UIInput>().text = string2;
		transform4.GetComponent<UIInput>().label.text = string2;
		transform3.GetComponent<UIInput>().label.shrinkToFit = true;
		transform4.GetComponent<UIInput>().label.shrinkToFit = true;
		Transform transform5 = transform.Find("LabelAuthPass");
		Transform transform6 = transform.Find("InputAuthPass");
		if (transform6 == null)
		{
			uint width = (uint)transform3.transform.Find("Background").localScale.x;
			Vector3 position = transform2.localPosition + new Vector3(0f, 61f, 0f);
			transform6 = CreateInput(transform.gameObject, transform3.gameObject, position, transform2.rotation, "InputAuthPass", string.Empty, width).transform;
			transform6.GetComponent<UIInput>().label.shrinkToFit = true;
		}
		if (transform5 == null)
		{
			Vector3 position = transform6.localPosition + new Vector3(0f, 35f, 0f);
			GameObject gameObject = transform.Find("LabelIP").gameObject;
			transform5 = CreateLabel(transform.gameObject, gameObject, position, transform6.rotation, "LabelAuthPass", "Admin Password (Optional)", gameObject.GetComponent<UILabel>().font.dynamicFontSize, gameObject.GetComponent<UILabel>().lineWidth).transform;
			transform5.localScale = gameObject.transform.localScale;
			transform5.GetComponent<UILabel>().color = gameObject.GetComponent<UILabel>().color;
		}
		string string3 = PlayerPrefs.GetString("lastAuthPass", string.Empty);
		transform6.GetComponent<UIInput>().text = string3;
		transform6.GetComponent<UIInput>().label.text = string3;
	}

	private void Start()
	{
	}
}
