using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Color")]
public class UIButtonColor : MonoBehaviour
{
	public float duration = 0.2f;

	public Color hover = new Color(0.6f, 1f, 0.2f, 1f);

	protected Color mColor;

	protected bool mHighlighted;

	protected bool mStarted;

	public Color pressed = Color.grey;

	public GameObject tweenTarget;

	public Color defaultColor
	{
		get
		{
			if (!mStarted)
			{
				Init();
			}
			return mColor;
		}
		set
		{
			mColor = value;
		}
	}

	protected void Init()
	{
		if (tweenTarget == null)
		{
			tweenTarget = base.gameObject;
		}
		UIWidget component = tweenTarget.GetComponent<UIWidget>();
		if (component != null)
		{
			mColor = component.color;
		}
		else
		{
			Renderer renderer = tweenTarget.renderer;
			if (renderer != null)
			{
				mColor = renderer.material.color;
			}
			else
			{
				Light light = tweenTarget.light;
				if (light != null)
				{
					mColor = light.color;
				}
				else
				{
					Debug.LogWarning(NGUITools.GetHierarchy(base.gameObject) + " has nothing for UIButtonColor to color", this);
					base.enabled = false;
				}
			}
		}
		OnEnable();
	}

	private void OnDisable()
	{
		if (mStarted && tweenTarget != null)
		{
			TweenColor component = tweenTarget.GetComponent<TweenColor>();
			if (component != null)
			{
				component.color = mColor;
				component.enabled = false;
			}
		}
	}

	protected virtual void OnEnable()
	{
		if (mStarted && mHighlighted)
		{
			OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	public virtual void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenColor.Begin(tweenTarget, duration, (!isOver) ? mColor : hover);
			mHighlighted = isOver;
		}
	}

	public virtual void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenColor.Begin(tweenTarget, duration, isPressed ? pressed : ((!UICamera.IsHighlighted(base.gameObject)) ? mColor : hover));
		}
	}

	private void Start()
	{
		if (!mStarted)
		{
			Init();
			mStarted = true;
		}
	}
}
