using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	public string disabledSprite;

	public string hoverSprite;

	public string normalSprite;

	public string pressedSprite;

	public UISprite target;

	public bool isEnabled
	{
		get
		{
			Collider collider = base.collider;
			if (collider != null)
			{
				return collider.enabled;
			}
			return false;
		}
		set
		{
			Collider collider = base.collider;
			if (collider != null && collider.enabled != value)
			{
				collider.enabled = value;
				UpdateImage();
			}
		}
	}

	private void Awake()
	{
		if (target == null)
		{
			target = GetComponentInChildren<UISprite>();
		}
	}

	private void OnEnable()
	{
		UpdateImage();
	}

	private void OnHover(bool isOver)
	{
		if (isEnabled && target != null)
		{
			target.spriteName = ((!isOver) ? normalSprite : hoverSprite);
			target.MakePixelPerfect();
		}
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			target.spriteName = pressedSprite;
			target.MakePixelPerfect();
		}
		else
		{
			UpdateImage();
		}
	}

	private void UpdateImage()
	{
		if (target != null)
		{
			if (isEnabled)
			{
				target.spriteName = ((!UICamera.IsHighlighted(base.gameObject)) ? normalSprite : hoverSprite);
			}
			else
			{
				target.spriteName = disabledSprite;
			}
			target.MakePixelPerfect();
		}
	}
}
