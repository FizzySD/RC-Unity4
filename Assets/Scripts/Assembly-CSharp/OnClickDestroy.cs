using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
	public bool DestroyByRpc;

	[RPC]
	public void DestroyRpc()
	{
		Object.Destroy(base.gameObject);
		PhotonNetwork.UnAllocateViewID(base.photonView.viewID);
	}

	private void OnClick()
	{
		if (!DestroyByRpc)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			base.photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered);
		}
	}
}
