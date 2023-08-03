using UnityEngine;

[AddComponentMenu("NGUI/Examples/Look At Target")]
public class LookAtTarget : MonoBehaviour
{
	public int level;

	private Transform mTrans;

	public float speed = 8f;

	public Transform target;

	private void LateUpdate()
	{
		if (target != null)
		{
			Vector3 forward = target.position - mTrans.position;
			if (forward.magnitude > 0.001f)
			{
				Quaternion to = Quaternion.LookRotation(forward);
				mTrans.rotation = Quaternion.Slerp(mTrans.rotation, to, Mathf.Clamp01(speed * Time.deltaTime));
			}
		}
	}

	private void Start()
	{
		mTrans = base.transform;
	}
}
