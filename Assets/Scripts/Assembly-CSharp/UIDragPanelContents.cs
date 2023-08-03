using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Panel Contents")]
[ExecuteInEditMode]
public class UIDragPanelContents : MonoBehaviour
{
	public UIDraggablePanel draggablePanel;

	[HideInInspector]
	[SerializeField]
	private UIPanel panel;

	private void Awake()
	{
		if (!(panel != null))
		{
			return;
		}
		if (draggablePanel == null)
		{
			draggablePanel = panel.GetComponent<UIDraggablePanel>();
			if (draggablePanel == null)
			{
				draggablePanel = panel.gameObject.AddComponent<UIDraggablePanel>();
			}
		}
		panel = null;
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggablePanel != null)
		{
			draggablePanel.Drag();
		}
	}

	private void OnPress(bool pressed)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggablePanel != null)
		{
			draggablePanel.Press(pressed);
		}
	}

	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggablePanel != null)
		{
			draggablePanel.Scroll(delta);
		}
	}

	private void Start()
	{
		if (draggablePanel == null)
		{
			draggablePanel = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
		}
	}
}
