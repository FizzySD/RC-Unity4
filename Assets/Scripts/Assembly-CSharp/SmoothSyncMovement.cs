using Photon;
using UnityEngine;

public class SmoothSyncMovement : Photon.MonoBehaviour
{
	private Vector3 correctCameraPos;

	public Quaternion correctCameraRot;

	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;

	private Vector3 correctPlayerVelocity = Vector3.zero;

	public bool disabled;

	public bool noVelocity;

	public bool PhotonCamera;

	public float SmoothingDelay = 5f;

	public void Awake()
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			base.enabled = false;
		}
		correctPlayerPos = base.transform.position;
		correctPlayerRot = base.transform.rotation;
		if (base.rigidbody == null)
		{
			noVelocity = true;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			if (!noVelocity)
			{
				stream.SendNext(base.rigidbody.velocity);
			}
			if (PhotonCamera)
			{
				stream.SendNext(Camera.main.transform.rotation);
			}
		}
		else
		{
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			if (!noVelocity)
			{
				correctPlayerVelocity = (Vector3)stream.ReceiveNext();
			}
			if (PhotonCamera)
			{
				correctCameraRot = (Quaternion)stream.ReceiveNext();
			}
		}
	}

	public void Update()
	{
		if (!disabled && !base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, correctPlayerPos, Time.deltaTime * SmoothingDelay);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, correctPlayerRot, Time.deltaTime * SmoothingDelay);
			if (!noVelocity)
			{
				base.rigidbody.velocity = correctPlayerVelocity;
			}
		}
	}
}
