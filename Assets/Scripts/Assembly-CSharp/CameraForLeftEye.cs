using UnityEngine;

public class CameraForLeftEye : MonoBehaviour
{
	private new Camera camera;

	private Camera cameraRightEye;

	public GameObject rightEye;

	private void LateUpdate()
	{
		camera.aspect = cameraRightEye.aspect;
		camera.fieldOfView = cameraRightEye.fieldOfView;
	}

	private void Start()
	{
		camera = GetComponent<Camera>();
		cameraRightEye = rightEye.GetComponent<Camera>();
	}
}
