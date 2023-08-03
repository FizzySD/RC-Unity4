using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIButtonScale : MonoBehaviour
{
	public float duration = 0.2f;

	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);

	private bool mHighlighted;

	private Vector3 mScale;

	private bool mStarted;

	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);

	public Transform tweenTarget;

	private void OnDisable()
	{
		if (mStarted && tweenTarget != null)
		{
			TweenScale component = tweenTarget.GetComponent<TweenScale>();
			if (component != null)
			{
				component.scale = mScale;
				component.enabled = false;
			}
		}
	}

	private void OnEnable()
	{
		if (mStarted && mHighlighted)
		{
			OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	private void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenScale.Begin(tweenTarget.gameObject, duration, (!isOver) ? mScale : Vector3.Scale(mScale, hover)).method = UITweener.Method.EaseInOut;
			mHighlighted = isOver;
		}
	}

	private void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenScale.Begin(tweenTarget.gameObject, duration, isPressed ? Vector3.Scale(mScale, pressed) : ((!UICamera.IsHighlighted(base.gameObject)) ? mScale : Vector3.Scale(mScale, hover))).method = UITweener.Method.EaseInOut;
		}
	}

	private void Start()
	{
		if (!mStarted)
		{
			mStarted = true;
			if (tweenTarget == null)
			{
				tweenTarget = base.transform;
			}
			mScale = tweenTarget.localScale;
		}
	}
}
