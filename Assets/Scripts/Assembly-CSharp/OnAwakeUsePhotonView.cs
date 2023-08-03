using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnAwakeUsePhotonView : Photon.MonoBehaviour
{
	private void Awake()
	{
		if (base.photonView.isMine)
		{
			base.photonView.RPC("OnAwakeRPC", PhotonTargets.All);
		}
	}

	[RPC]
	public void OnAwakeRPC()
	{
		PhotonView obj = base.photonView;
		Debug.Log("RPC: 'OnAwakeRPC' PhotonView: " + (((object)obj != null) ? obj.ToString() : null));
	}

	[RPC]
	public void OnAwakeRPC(byte myParameter)
	{
		Debug.Log("RPC: 'OnAwakeRPC' Parameter: " + myParameter + " PhotonView: " + base.photonView);
	}

	private void Start()
	{
		if (base.photonView.isMine)
		{
			object[] parameters = new object[1] { (byte)1 };
			base.photonView.RPC("OnAwakeRPC", PhotonTargets.All, parameters);
		}
	}
}
