using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Camera")]
[ExecuteInEditMode]
public class UIDragCamera : IgnoreTimeScale
{
	public UIDraggableCamera draggableCamera;

	[SerializeField]
	[HideInInspector]
	private Component target;

	private void Awake()
	{
		if (target != null)
		{
			if (draggableCamera == null)
			{
				draggableCamera = target.GetComponent<UIDraggableCamera>();
				if (draggableCamera == null)
				{
					draggableCamera = target.gameObject.AddComponent<UIDraggableCamera>();
				}
			}
			target = null;
		}
		else if (draggableCamera == null)
		{
			draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(base.gameObject);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null)
		{
			draggableCamera.Drag(delta);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null)
		{
			draggableCamera.Press(isPressed);
		}
	}

	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null)
		{
			draggableCamera.Scroll(delta);
		}
	}
}
