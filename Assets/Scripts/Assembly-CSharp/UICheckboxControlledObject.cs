using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Object")]
public class UICheckboxControlledObject : MonoBehaviour
{
	public bool inverse;

	public GameObject target;

	private void OnActivate(bool isActive)
	{
		if (target != null)
		{
			NGUITools.SetActive(target, (!inverse) ? isActive : (!isActive));
			UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(target);
			if (uIPanel != null)
			{
				uIPanel.Refresh();
			}
		}
	}

	private void OnEnable()
	{
		UICheckbox component = GetComponent<UICheckbox>();
		if (component != null)
		{
			OnActivate(component.isChecked);
		}
	}
}
