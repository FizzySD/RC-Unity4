using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
	public enum Axis
	{
		up = 0,
		down = 1,
		left = 2,
		right = 3,
		forward = 4,
		back = 5
	}

	public Axis axis;

	private Camera referenceCamera;

	public bool reverseFace;

	private void Awake()
	{
		if (referenceCamera == null)
		{
			referenceCamera = Camera.main;
		}
	}

	public Vector3 GetAxis(Axis refAxis)
	{
		switch (refAxis)
		{
		case Axis.down:
			return Vector3.down;
		case Axis.left:
			return Vector3.left;
		case Axis.right:
			return Vector3.right;
		case Axis.forward:
			return Vector3.forward;
		case Axis.back:
			return Vector3.back;
		default:
			return Vector3.up;
		}
	}

	private void Update()
	{
		Vector3 worldPosition = base.transform.position + referenceCamera.transform.rotation * ((!reverseFace) ? Vector3.back : Vector3.forward);
		Vector3 worldUp = referenceCamera.transform.rotation * GetAxis(axis);
		base.transform.LookAt(worldPosition, worldUp);
	}
}
