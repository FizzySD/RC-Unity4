using UnityEngine;

public class MovementUpdate : MonoBehaviour
{
	public bool disabled;

	private Vector3 lastPosition;

	private Quaternion lastRotation;

	private Vector3 lastVelocity;

	private Vector3 targetPosition;

	private void Start()
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			disabled = true;
			base.enabled = false;
		}
		else if (base.networkView.isMine)
		{
			object[] args = new object[4]
			{
				base.transform.position,
				base.transform.rotation,
				base.transform.localScale,
				Vector3.zero
			};
			base.networkView.RPC("updateMovement", RPCMode.OthersBuffered, args);
		}
		else
		{
			targetPosition = base.transform.position;
		}
	}

	private void Update()
	{
		if (disabled || Network.peerType == NetworkPeerType.Disconnected || Network.peerType == NetworkPeerType.Connecting)
		{
			return;
		}
		if (base.networkView.isMine)
		{
			if (Vector3.Distance(base.transform.position, lastPosition) >= 0.5f)
			{
				lastPosition = base.transform.position;
				object[] args = new object[4]
				{
					base.transform.position,
					base.transform.rotation,
					base.transform.localScale,
					base.rigidbody.velocity
				};
				base.networkView.RPC("updateMovement", RPCMode.Others, args);
			}
			else if (Vector3.Distance(base.transform.rigidbody.velocity, lastVelocity) >= 0.1f)
			{
				lastVelocity = base.transform.rigidbody.velocity;
				object[] args2 = new object[4]
				{
					base.transform.position,
					base.transform.rotation,
					base.transform.localScale,
					base.rigidbody.velocity
				};
				base.networkView.RPC("updateMovement", RPCMode.Others, args2);
			}
			else if (Quaternion.Angle(base.transform.rotation, lastRotation) >= 1f)
			{
				lastRotation = base.transform.rotation;
				object[] args3 = new object[4]
				{
					base.transform.position,
					base.transform.rotation,
					base.transform.localScale,
					base.rigidbody.velocity
				};
				base.networkView.RPC("updateMovement", RPCMode.Others, args3);
			}
		}
		else
		{
			base.transform.position = Vector3.Slerp(base.transform.position, targetPosition, Time.deltaTime * 2f);
		}
	}

	[RPC]
	private void updateMovement(Vector3 newPosition, Quaternion newRotation, Vector3 newScale, Vector3 veloctiy)
	{
		targetPosition = newPosition;
		base.transform.rotation = newRotation;
		base.transform.localScale = newScale;
		base.rigidbody.velocity = veloctiy;
	}
}
