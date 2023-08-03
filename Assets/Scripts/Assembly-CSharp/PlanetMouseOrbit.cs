using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit")]
public class PlanetMouseOrbit : MonoBehaviour
{
	public float distance = 10f;

	public Transform target;

	private float x;

	public float xSpeed = 250f;

	private float y;

	public int yMaxLimit = 80;

	public int yMinLimit = -20;

	public float ySpeed = 120f;

	public int zoomRate = 25;

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public void Main()
	{
	}

	public void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
	}

	public void Update()
	{
		if (target != null)
		{
			x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			distance += (0f - Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * (float)zoomRate * Mathf.Abs(distance);
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			Quaternion quaternion = Quaternion.Euler(y, x, 0f);
			Vector3 position = quaternion * new Vector3(0f, 0f, 0f - distance) + target.position;
			base.transform.rotation = quaternion;
			base.transform.position = position;
		}
	}
}
