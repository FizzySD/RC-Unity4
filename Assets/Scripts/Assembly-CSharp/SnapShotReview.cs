using System;
using ApplicationManagers;
using UnityEngine;

public class SnapShotReview : MonoBehaviour
{
	public GameObject labelDMG;

	public GameObject labelInfo;

	public GameObject labelPage;

	private UILabel page;

	public GameObject texture;

	private float textureH = 600f;

	private float textureW = 960f;

	private int _currentIndex;

	private void freshInfo()
	{
		if (SnapshotManager.GetLength() == 0)
		{
			page.text = "0/0";
		}
		else
		{
			page.text = _currentIndex + 1 + "/" + SnapshotManager.GetLength();
		}
		if (SnapshotManager.GetDamage(_currentIndex) > 0)
		{
			labelDMG.GetComponent<UILabel>().text = SnapshotManager.GetDamage(_currentIndex).ToString();
		}
		else
		{
			labelDMG.GetComponent<UILabel>().text = string.Empty;
		}
	}

	private void setTextureWH()
	{
		if (SnapshotManager.GetLength() != 0)
		{
			float num = 1.6f;
			float num2 = (float)texture.GetComponent<UITexture>().mainTexture.width / (float)texture.GetComponent<UITexture>().mainTexture.height;
			if (num2 > num)
			{
				texture.transform.localScale = new Vector3(textureW, textureW / num2, 0f);
				labelDMG.transform.localPosition = new Vector3((int)(textureW * 0.5f - 20f), (int)(0f + textureW * 0.5f / num2 - 20f), -20f);
				labelInfo.transform.localPosition = new Vector3((int)(textureW * 0.5f - 20f), (int)(0f - textureW * 0.5f / num2 + 20f), -20f);
			}
			else
			{
				texture.transform.localScale = new Vector3(textureH * num2, textureH, 0f);
				labelDMG.transform.localPosition = new Vector3((int)(textureH * num2 * 0.5f - 20f), (int)(0f + textureH * 0.5f - 20f), -20f);
				labelInfo.transform.localPosition = new Vector3((int)(textureH * num2 * 0.5f - 20f), (int)(0f - textureH * 0.5f + 20f), -20f);
			}
		}
	}

	public void ShowNextIMG()
	{
		if (_currentIndex < SnapshotManager.GetLength() - 1)
		{
			_currentIndex++;
			texture.GetComponent<UITexture>().mainTexture = SnapshotManager.GetSnapshot(_currentIndex);
			setTextureWH();
			freshInfo();
		}
	}

	public void ShowPrevIMG()
	{
		if (_currentIndex > 0)
		{
			_currentIndex--;
			texture.GetComponent<UITexture>().mainTexture = SnapshotManager.GetSnapshot(_currentIndex);
			setTextureWH();
			freshInfo();
		}
	}

	private void Start()
	{
		page = labelPage.GetComponent<UILabel>();
		_currentIndex = 0;
		if (SnapshotManager.GetLength() > 0)
		{
			texture.GetComponent<UITexture>().mainTexture = SnapshotManager.GetSnapshot(_currentIndex);
		}
		labelInfo.GetComponent<UILabel>().text = LoginFengKAI.player.name + " " + DateTime.Today.ToShortDateString();
		freshInfo();
		setTextureWH();
	}
}
