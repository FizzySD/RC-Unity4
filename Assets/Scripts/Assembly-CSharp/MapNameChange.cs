using UnityEngine;

public class MapNameChange : MonoBehaviour
{
	private void OnSelectionChange()
	{
		LevelInfo info = LevelInfo.getInfo(GetComponent<UIPopupList>().selection);
		if (info != null)
		{
			GameObject.Find("LabelLevelInfo").GetComponent<UILabel>().text = info.desc;
		}
		if (!GetComponent<UIPopupList>().items.Contains("Custom"))
		{
			GetComponent<UIPopupList>().items.Add("Custom");
			UIPopupList component = GetComponent<UIPopupList>();
			component.textScale *= 0.8f;
		}
		if (!GetComponent<UIPopupList>().items.Contains("Custom (No PT)"))
		{
			GetComponent<UIPopupList>().items.Add("Custom (No PT)");
		}
	}
}
