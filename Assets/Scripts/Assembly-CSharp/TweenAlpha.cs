using UnityEngine;

[AddComponentMenu("NGUI/Tween/Alpha")]
public class TweenAlpha : UITweener
{
	public float from = 1f;

	private UIPanel mPanel;

	private Transform mTrans;

	private UIWidget mWidget;

	public float to = 1f;

	public float alpha
	{
		get
		{
			if (mWidget != null)
			{
				return mWidget.alpha;
			}
			if (mPanel != null)
			{
				return mPanel.alpha;
			}
			return 0f;
		}
		set
		{
			if (mWidget != null)
			{
				mWidget.alpha = value;
			}
			else if (mPanel != null)
			{
				mPanel.alpha = value;
			}
		}
	}

	private void Awake()
	{
		mPanel = GetComponent<UIPanel>();
		if (mPanel == null)
		{
			mWidget = GetComponentInChildren<UIWidget>();
		}
	}

	public static TweenAlpha Begin(GameObject go, float duration, float alpha)
	{
		TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration);
		tweenAlpha.from = tweenAlpha.alpha;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, true);
			tweenAlpha.enabled = false;
		}
		return tweenAlpha;
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		alpha = Mathf.Lerp(from, to, factor);
	}
}
