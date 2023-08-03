using UnityEngine;

[AddComponentMenu("NGUI/Examples/Lag Rotation")]
public class LagRotation : MonoBehaviour
{
	public bool ignoreTimeScale;

	private Quaternion mAbsolute;

	private Quaternion mRelative;

	private Transform mTrans;

	public float speed = 10f;

	public int updateOrder;

	private void CoroutineUpdate(float delta)
	{
		Transform parent = mTrans.parent;
		if (parent != null)
		{
			mAbsolute = Quaternion.Slerp(mAbsolute, parent.rotation * mRelative, delta * speed);
			mTrans.rotation = mAbsolute;
		}
	}

	private void Start()
	{
		mTrans = base.transform;
		mRelative = mTrans.localRotation;
		mAbsolute = mTrans.rotation;
		if (ignoreTimeScale)
		{
			UpdateManager.AddCoroutine(this, updateOrder, CoroutineUpdate);
		}
		else
		{
			UpdateManager.AddLateUpdate(this, updateOrder, CoroutineUpdate);
		}
	}
}
