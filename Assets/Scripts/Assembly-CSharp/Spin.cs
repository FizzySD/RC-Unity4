using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin")]
public class Spin : MonoBehaviour
{
	private Rigidbody mRb;

	private Transform mTrans;

	public Vector3 rotationsPerSecond = new Vector3(0f, 0.1f, 0f);

	public void ApplyDelta(float delta)
	{
		delta *= 360f;
		Quaternion quaternion = Quaternion.Euler(rotationsPerSecond * delta);
		if (mRb == null)
		{
			mTrans.rotation *= quaternion;
		}
		else
		{
			mRb.MoveRotation(mRb.rotation * quaternion);
		}
	}

	private void FixedUpdate()
	{
		if (mRb != null)
		{
			ApplyDelta(Time.deltaTime);
		}
	}

	private void Start()
	{
		mTrans = base.transform;
		mRb = base.rigidbody;
	}

	private void Update()
	{
		if (mRb == null)
		{
			ApplyDelta(Time.deltaTime);
		}
	}
}
