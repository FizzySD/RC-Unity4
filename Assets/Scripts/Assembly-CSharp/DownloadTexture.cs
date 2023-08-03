using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
	private Material mMat;

	private Texture2D mTex;

	public string url = "http://www.tasharen.com/misc/logo.png";

	private void OnDestroy()
	{
		if (mMat != null)
		{
			Object.Destroy(mMat);
		}
		if (mTex != null)
		{
			Object.Destroy(mTex);
		}
	}

	private IEnumerator Start()
	{
		WWW wWW = new WWW(url);
		yield return wWW;
		mTex = wWW.texture;
		if (!(mTex == null))
		{
			UITexture component = GetComponent<UITexture>();
			if (component.material != null)
			{
				mMat = new Material(component.material);
			}
			else
			{
				mMat = new Material(Shader.Find("Unlit/Transparent Colored"));
			}
			component.material = mMat;
			mMat.mainTexture = mTex;
			component.MakePixelPerfect();
		}
		wWW.Dispose();
	}
}
