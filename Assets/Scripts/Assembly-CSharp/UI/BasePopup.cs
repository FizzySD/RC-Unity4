using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	internal class BasePopup : HeadedPanel
	{
		protected float _currentAnimationValue;

		protected HashSet<Transform> _staticTransforms = new HashSet<Transform>();

		protected virtual float MinTweenScale
		{
			get
			{
				return 0.3f;
			}
		}

		protected virtual float MaxTweenScale
		{
			get
			{
				return 1f;
			}
		}

		protected virtual float MinFadeAlpha
		{
			get
			{
				return 0f;
			}
		}

		protected virtual float MaxFadeAlpha
		{
			get
			{
				return 1f;
			}
		}

		protected virtual float AnimationTime
		{
			get
			{
				return 0.1f;
			}
		}

		protected virtual PopupAnimation PopupAnimationType
		{
			get
			{
				return PopupAnimation.Tween;
			}
		}

		public override void Show()
		{
			if (!base.gameObject.activeSelf)
			{
				base.Show();
				base.transform.SetAsLastSibling();
				StopAllCoroutines();
				if (PopupAnimationType == PopupAnimation.Tween)
				{
					StartCoroutine(TweenIn());
				}
				else if (PopupAnimationType == PopupAnimation.Fade)
				{
					StartCoroutine(FadeIn());
				}
			}
		}

		public override void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				HideAllPopups();
				StopAllCoroutines();
				if (PopupAnimationType == PopupAnimation.Tween)
				{
					StartCoroutine(TweenOut());
				}
				else if (PopupAnimationType == PopupAnimation.Fade)
				{
					StartCoroutine(FadeOut());
				}
				else if (PopupAnimationType == PopupAnimation.None)
				{
					FinishHide();
				}
			}
		}

		protected virtual void FinishHide()
		{
			base.gameObject.SetActive(false);
		}

		protected IEnumerator TweenIn()
		{
			_currentAnimationValue = MinTweenScale;
			while (_currentAnimationValue < MaxTweenScale)
			{
				SetTransformScale(_currentAnimationValue);
				_currentAnimationValue += GetAnimmationSpeed(MinTweenScale, MaxTweenScale) * Time.unscaledDeltaTime;
				yield return null;
			}
			SetTransformScale(MaxTweenScale);
		}

		protected IEnumerator TweenOut()
		{
			_currentAnimationValue = MaxTweenScale;
			while (_currentAnimationValue > MinTweenScale)
			{
				SetTransformScale(_currentAnimationValue);
				_currentAnimationValue -= GetAnimmationSpeed(MinTweenScale, MaxTweenScale) * Time.unscaledDeltaTime;
				yield return null;
			}
			SetTransformScale(MinTweenScale);
			FinishHide();
		}

		protected IEnumerator FadeIn()
		{
			_currentAnimationValue = MinFadeAlpha;
			while (_currentAnimationValue < MaxFadeAlpha)
			{
				SetTransformAlpha(_currentAnimationValue);
				_currentAnimationValue += GetAnimmationSpeed(MinFadeAlpha, MaxFadeAlpha) * Time.unscaledDeltaTime;
				yield return null;
			}
			SetTransformAlpha(MaxFadeAlpha);
		}

		protected IEnumerator FadeOut()
		{
			_currentAnimationValue = MaxFadeAlpha;
			while (_currentAnimationValue > MinFadeAlpha)
			{
				SetTransformAlpha(_currentAnimationValue);
				_currentAnimationValue -= GetAnimmationSpeed(MinFadeAlpha, MaxFadeAlpha) * Time.unscaledDeltaTime;
				yield return null;
			}
			SetTransformAlpha(MinFadeAlpha);
			FinishHide();
		}

		protected void SetTransformScale(float scale)
		{
			base.transform.localScale = GetVectorFromScale(scale);
			foreach (Transform staticTransform in _staticTransforms)
			{
				float num = 1f;
				IgnoreScaler component = staticTransform.GetComponent<IgnoreScaler>();
				if (component != null)
				{
					num = component.Scale;
				}
				staticTransform.localScale = GetVectorFromScale(num / Mathf.Max(scale, 0.1f));
			}
		}

		protected void SetTransformAlpha(float alpha)
		{
			CanvasGroup component = base.transform.GetComponent<CanvasGroup>();
			component.alpha = alpha;
		}

		private Vector3 GetVectorFromScale(float scale)
		{
			return new Vector3(scale, scale, scale);
		}

		private float GetAnimmationSpeed(float min, float max)
		{
			return (max - min) / AnimationTime;
		}
	}
}
