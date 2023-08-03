using System;
using System.Collections;
using ApplicationManagers;
using UnityEngine;

public class BTN_save_snapshot : MonoBehaviour
{
	public GameObject info;

	public GameObject targetTexture;

	public GameObject[] thingsNeedToHide;

	private void Awake()
	{
		info.GetComponent<UILabel>().text = string.Empty;
	}

	private void OnClick()
	{
		GameObject[] array = thingsNeedToHide;
		foreach (GameObject gameObject in array)
		{
			gameObject.transform.position += Vector3.up * 10000f;
		}
		StartCoroutine(ScreenshotEncode());
		info.GetComponent<UILabel>().text = "Saving...";
	}

	private IEnumerator ScreenshotEncode()
	{
		yield return new WaitForEndOfFrame();
		float num = (float)Screen.height / 600f;
		Texture2D texture2D = new Texture2D((int)(num * targetTexture.transform.localScale.x), (int)(num * targetTexture.transform.localScale.y), TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect((float)Screen.width * 0.5f - (float)texture2D.width * 0.5f, (float)Screen.height * 0.5f - (float)texture2D.height * 0.5f - num * 0f, texture2D.width, texture2D.height), 0, 0);
		texture2D.Apply();
		yield return 0;
		GameObject[] array = thingsNeedToHide;
		foreach (GameObject gameObject in array)
		{
			gameObject.transform.position -= Vector3.up * 10000f;
		}
		string[] array2 = new string[8]
		{
			DateTime.Today.Month.ToString(),
			DateTime.Today.Day.ToString(),
			DateTime.Today.Year.ToString(),
			"-",
			DateTime.Now.Hour.ToString(),
			DateTime.Now.Minute.ToString(),
			DateTime.Now.Second.ToString(),
			".png"
		};
		string text = string.Concat(array2);
		object[] array3 = new object[4]
		{
			text,
			texture2D.width,
			texture2D.height,
			Convert.ToBase64String(texture2D.EncodeToPNG())
		};
		SnapshotManager.SaveSnapshotFinish(texture2D, text);
		UnityEngine.Object.DestroyObject(texture2D);
		info.GetComponent<UILabel>().text = string.Format("Saved snapshot to {0}", SnapshotManager.SnapshotPath);
	}
}
