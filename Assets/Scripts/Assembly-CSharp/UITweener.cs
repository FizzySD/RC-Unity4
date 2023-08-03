using AnimationOrTween;
using UnityEngine;

public abstract class UITweener : IgnoreTimeScale
{
	public enum Method
	{
		Linear = 0,
		EaseIn = 1,
		EaseOut = 2,
		EaseInOut = 3,
		BounceIn = 4,
		BounceOut = 5
	}

	public delegate void OnFinished(UITweener tween);

	public enum Style
	{
		Once = 0,
		Loop = 1,
		PingPong = 2
	}

	public AnimationCurve animationCurve;

	public string callWhenFinished;

	public float delay;

	public float duration;

	public GameObject eventReceiver;

	public bool ignoreTimeScale;

	private float mAmountPerDelta;

	private float mDuration;

	public Method method;

	private float mFactor;

	private bool mStarted;

	private float mStartTime;

	public OnFinished onFinished;

	public bool steeperCurves;

	public Style style;

	public int tweenGroup;

	public float amountPerDelta
	{
		get
		{
			if (mDuration != duration)
			{
				mDuration = duration;
				mAmountPerDelta = Mathf.Abs((duration <= 0f) ? 1000f : (1f / duration));
			}
			return mAmountPerDelta;
		}
	}

	public Direction direction
	{
		get
		{
			if (!(mAmountPerDelta >= 0f))
			{
				return Direction.Reverse;
			}
			return Direction.Forward;
		}
	}

	public float tweenFactor
	{
		get
		{
			return mFactor;
		}
	}

	protected UITweener()
	{
		Keyframe[] keys = new Keyframe[2]
		{
			new Keyframe(0f, 0f, 0f, 1f),
			new Keyframe(1f, 1f, 1f, 0f)
		};
		animationCurve = new AnimationCurve(keys);
		ignoreTimeScale = true;
		duration = 1f;
		mAmountPerDelta = 1f;
	}

	public static T Begin<T>(GameObject go, float duration) where T : UITweener
	{
		T val = go.GetComponent<T>();
		if ((Object)val == (Object)null)
		{
			val = go.AddComponent<T>();
		}
		val.mStarted = false;
		val.duration = duration;
		val.mFactor = 0f;
		val.mAmountPerDelta = Mathf.Abs(val.mAmountPerDelta);
		val.style = Style.Once;
		Keyframe[] keys = new Keyframe[2]
		{
			new Keyframe(0f, 0f, 0f, 1f),
			new Keyframe(1f, 1f, 1f, 0f)
		};
		val.animationCurve = new AnimationCurve(keys);
		val.eventReceiver = null;
		val.callWhenFinished = null;
		val.onFinished = null;
		val.enabled = true;
		return val;
	}

	private float BounceLogic(float val)
	{
		if (val < 0.363636f)
		{
			val = 7.5685f * val * val;
			return val;
		}
		if (val < 0.727272f)
		{
			val = 7.5625f * (val -= 0.545454f) * val + 0.75f;
			return val;
		}
		if (val < 0.90909f)
		{
			val = 7.5625f * (val -= 0.818181f) * val + 0.9375f;
			return val;
		}
		val = 7.5625f * (val -= 0.9545454f) * val + 63f / 64f;
		return val;
	}

	private void OnDisable()
	{
		mStarted = false;
	}

	protected abstract void OnUpdate(float factor, bool isFinished);

	public void Play(bool forward)
	{
		mAmountPerDelta = Mathf.Abs(amountPerDelta);
		if (!forward)
		{
			mAmountPerDelta = 0f - mAmountPerDelta;
		}
		base.enabled = true;
	}

	public void Reset()
	{
		mStarted = false;
		mFactor = ((mAmountPerDelta >= 0f) ? 0f : 1f);
		Sample(mFactor, false);
	}

	public void Sample(float factor, bool isFinished)
	{
		float num = Mathf.Clamp01(factor);
		if (method == Method.EaseIn)
		{
			num = 1f - Mathf.Sin(1.570796f * (1f - num));
			if (steeperCurves)
			{
				num *= num;
			}
		}
		else if (method == Method.EaseOut)
		{
			num = Mathf.Sin(1.570796f * num);
			if (steeperCurves)
			{
				num = 1f - num;
				num = 1f - num * num;
			}
		}
		else if (method == Method.EaseInOut)
		{
			num -= Mathf.Sin(num * 6.283185f) / 6.283185f;
			if (steeperCurves)
			{
				num = num * 2f - 1f;
				float num2 = Mathf.Sign(num);
				num = 1f - Mathf.Abs(num);
				num = 1f - num * num;
				num = num2 * num * 0.5f + 0.5f;
			}
		}
		else if (method == Method.BounceIn)
		{
			num = BounceLogic(num);
		}
		else if (method == Method.BounceOut)
		{
			num = 1f - BounceLogic(1f - num);
		}
		OnUpdate((animationCurve == null) ? num : animationCurve.Evaluate(num), isFinished);
	}

	private void Start()
	{
		Update();
	}

	public void Toggle()
	{
		if (mFactor > 0f)
		{
			mAmountPerDelta = 0f - amountPerDelta;
		}
		else
		{
			mAmountPerDelta = Mathf.Abs(amountPerDelta);
		}
		base.enabled = true;
	}

	private void Update()
	{
		float num = ((!ignoreTimeScale) ? Time.deltaTime : UpdateRealTimeDelta());
		float num2 = ((!ignoreTimeScale) ? Time.time : base.realTime);
		if (!mStarted)
		{
			mStarted = true;
			mStartTime = num2 + delay;
		}
		if (!(num2 >= mStartTime))
		{
			return;
		}
		mFactor += amountPerDelta * num;
		if (style == Style.Loop)
		{
			if (mFactor > 1f)
			{
				mFactor -= Mathf.Floor(mFactor);
			}
		}
		else if (style == Style.PingPong)
		{
			if (mFactor > 1f)
			{
				mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
				mAmountPerDelta = 0f - mAmountPerDelta;
			}
			else if (mFactor < 0f)
			{
				mFactor = 0f - mFactor;
				mFactor -= Mathf.Floor(mFactor);
				mAmountPerDelta = 0f - mAmountPerDelta;
			}
		}
		if (style == Style.Once && (mFactor > 1f || mFactor < 0f))
		{
			mFactor = Mathf.Clamp01(mFactor);
			Sample(mFactor, true);
			if (onFinished != null)
			{
				onFinished(this);
			}
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
			{
				eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
			}
			if ((mFactor == 1f && mAmountPerDelta > 0f) || (mFactor == 0f && mAmountPerDelta < 0f))
			{
				base.enabled = false;
			}
		}
		else
		{
			Sample(mFactor, false);
		}
	}
}
