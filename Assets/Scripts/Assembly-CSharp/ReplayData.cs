using UnityEngine;

public class ReplayData
{
	public bool isDashing;

	public Vector3 position { get; private set; }

	public Quaternion rotation { get; private set; }

	public string animId { get; private set; }

	public bool isGhostBoosting { get; private set; }

	public Vector3 LeftHookPos { get; private set; }

	public Vector3 RightHookPos { get; private set; }

	public ReplayData(Vector3 position, Quaternion rotation, string anim, Vector3 LeftHookPos, Vector3 RightHookPos, bool isGhostBoosting, bool isDashing)
	{
		this.position = position;
		this.rotation = rotation;
		animId = anim;
		this.LeftHookPos = LeftHookPos;
		this.RightHookPos = RightHookPos;
		this.isGhostBoosting = isGhostBoosting;
		this.isDashing = isDashing;
	}
}
