using UnityEngine;

public class InputToEvent : MonoBehaviour
{
	public bool DetectPointedAtGameObject;

	public static Vector3 inputHitPos;

	private GameObject lastGo;

	public static GameObject goPointedAt { get; private set; }

	private void Press(Vector2 screenPos)
	{
		lastGo = RaycastObject(screenPos);
		if (lastGo != null)
		{
			lastGo.SendMessage("OnPress", SendMessageOptions.DontRequireReceiver);
		}
	}

	private GameObject RaycastObject(Vector2 screenPos)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.camera.ScreenPointToRay(screenPos), out hitInfo, 200f))
		{
			inputHitPos = hitInfo.point;
			return hitInfo.collider.gameObject;
		}
		return null;
	}

	private void Release(Vector2 screenPos)
	{
		if (lastGo != null)
		{
			if (RaycastObject(screenPos) == lastGo)
			{
				lastGo.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			lastGo.SendMessage("OnRelease", SendMessageOptions.DontRequireReceiver);
			lastGo = null;
		}
	}

	private void Update()
	{
		if (DetectPointedAtGameObject)
		{
			goPointedAt = RaycastObject(Input.mousePosition);
		}
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				Press(touch.position);
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				Release(touch.position);
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				Press(Input.mousePosition);
			}
			if (Input.GetMouseButtonUp(0))
			{
				Release(Input.mousePosition);
			}
		}
	}
}
