using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ShowInfoOfPlayer : Photon.MonoBehaviour
{
	public bool DisableOnOwnObjects;

	public Font font;

	private const int FontSize3D = 0;

	private GameObject textGo;

	private TextMesh tm;

	private void OnDisable()
	{
		if (textGo != null)
		{
			textGo.SetActive(false);
		}
	}

	private void OnEnable()
	{
		if (textGo != null)
		{
			textGo.SetActive(true);
		}
	}

	private void Start()
	{
		if (font == null)
		{
			font = (Font)Resources.FindObjectsOfTypeAll(typeof(Font))[0];
			Font obj = font;
			Debug.LogWarning("No font defined. Found font: " + (((object)obj != null) ? obj.ToString() : null));
		}
		if (tm == null)
		{
			textGo = new GameObject("3d text");
			textGo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			textGo.transform.parent = base.gameObject.transform;
			textGo.transform.localPosition = Vector3.zero;
			textGo.AddComponent<MeshRenderer>().material = font.material;
			tm = textGo.AddComponent<TextMesh>();
			tm.font = font;
			tm.fontSize = 0;
			tm.anchor = TextAnchor.MiddleCenter;
		}
		if (!DisableOnOwnObjects && base.photonView.isMine)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (DisableOnOwnObjects)
		{
			base.enabled = false;
			if (textGo != null)
			{
				textGo.SetActive(false);
			}
			return;
		}
		PhotonPlayer owner = base.photonView.owner;
		if (owner != null)
		{
			tm.text = ((!string.IsNullOrEmpty(owner.name)) ? owner.name : "n/a");
		}
		else if (base.photonView.isSceneView)
		{
			if (!DisableOnOwnObjects && base.photonView.isMine)
			{
				base.enabled = false;
				textGo.SetActive(false);
			}
			else
			{
				tm.text = "scn";
			}
		}
		else
		{
			tm.text = "n/a";
		}
	}
}
