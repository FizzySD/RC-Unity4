using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/UI/Viewport Camera")]
[ExecuteInEditMode]
public class UIViewport : MonoBehaviour
{
	public Transform bottomRight;

	public float fullSize = 1f;

	private Camera mCam;

	public Camera sourceCamera;

	public Transform topLeft;

	private void LateUpdate()
	{
		if (topLeft != null && bottomRight != null)
		{
			Vector3 vector = sourceCamera.WorldToScreenPoint(topLeft.position);
			Vector3 vector2 = sourceCamera.WorldToScreenPoint(bottomRight.position);
			Rect rect = new Rect(vector.x / (float)Screen.width, vector2.y / (float)Screen.height, (vector2.x - vector.x) / (float)Screen.width, (vector.y - vector2.y) / (float)Screen.height);
			float num = fullSize * rect.height;
			if (rect != mCam.rect)
			{
				mCam.rect = rect;
			}
			if (mCam.orthographicSize != num)
			{
				mCam.orthographicSize = num;
			}
		}
	}

	private void Start()
	{
		mCam = base.camera;
		if (sourceCamera == null)
		{
			sourceCamera = Camera.main;
		}
	}
}
