using UnityEngine;

public class PanelGroupManager
{
	public GameObject[] panelGroup;

	public void ActivePanel(int index)
	{
		GameObject[] array = panelGroup;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		panelGroup[index].SetActive(true);
	}
}
