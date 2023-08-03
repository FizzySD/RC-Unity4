using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Play Animation")]
public class UIButtonPlayAnimation : MonoBehaviour
{
	public string callWhenFinished;

	public bool clearSelection;

	public string clipName;

	public DisableCondition disableWhenFinished;

	public GameObject eventReceiver;

	public EnableCondition ifDisabledOnPlay;

	private bool mHighlighted;

	private bool mStarted;

	public ActiveAnimation.OnFinished onFinished;

	public Direction playDirection = Direction.Forward;

	public bool resetOnPlay;

	public Animation target;

	public Trigger trigger;

	private void OnActivate(bool isActive)
	{
		if (base.enabled && (trigger == Trigger.OnActivate || (trigger == Trigger.OnActivateTrue && isActive) || (trigger == Trigger.OnActivateFalse && !isActive)))
		{
			Play(isActive);
		}
	}

	private void OnClick()
	{
		if (base.enabled && trigger == Trigger.OnClick)
		{
			Play(true);
		}
	}

	private void OnDoubleClick()
	{
		if (base.enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(true);
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
			if (trigger == Trigger.OnHover || (trigger == Trigger.OnHoverTrue && isOver) || (trigger == Trigger.OnHoverFalse && !isOver))
			{
				Play(isOver);
			}
			mHighlighted = isOver;
		}
	}

	private void OnPress(bool isPressed)
	{
		if (base.enabled && (trigger == Trigger.OnPress || (trigger == Trigger.OnPressTrue && isPressed) || (trigger == Trigger.OnPressFalse && !isPressed)))
		{
			Play(isPressed);
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (trigger == Trigger.OnSelect || (trigger == Trigger.OnSelectTrue && isSelected) || (trigger == Trigger.OnSelectFalse && !isSelected)))
		{
			Play(true);
		}
	}

	private void Play(bool forward)
	{
		if (target == null)
		{
			target = GetComponentInChildren<Animation>();
		}
		if (!(target != null))
		{
			return;
		}
		if (clearSelection && UICamera.selectedObject == base.gameObject)
		{
			UICamera.selectedObject = null;
		}
		int num = (int)playDirection;
		Direction direction = ((!forward) ? ((Direction)num) : playDirection);
		ActiveAnimation activeAnimation = ActiveAnimation.Play(target, clipName, direction, ifDisabledOnPlay, disableWhenFinished);
		if (activeAnimation != null)
		{
			if (resetOnPlay)
			{
				activeAnimation.Reset();
			}
			activeAnimation.onFinished = onFinished;
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
			{
				activeAnimation.eventReceiver = eventReceiver;
				activeAnimation.callWhenFinished = callWhenFinished;
			}
			else
			{
				activeAnimation.eventReceiver = null;
			}
		}
	}

	private void Start()
	{
		mStarted = true;
	}
}
