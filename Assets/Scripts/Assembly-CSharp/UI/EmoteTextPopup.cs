using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class EmoteTextPopup : BasePopup
	{
		private const float ShowTime = 3f;

		private Text _text;

		protected Transform _parent;

		protected float _currentShowTime;

		protected bool _isHiding;

		protected Transform _transform;

		protected Camera _camera;

		protected override float AnimationTime
		{
			get
			{
				return 0.25f;
			}
		}

		protected override PopupAnimation PopupAnimationType
		{
			get
			{
				return PopupAnimation.Fade;
			}
		}

		protected virtual Vector3 offset
		{
			get
			{
				return Vector3.up * 2.5f;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			_text = base.transform.Find("Panel/Text/Label").GetComponent<Text>();
			_transform = base.transform;
		}

		public void Show(string text, Transform parent)
		{
			_parent = parent;
			_currentShowTime = 3f;
			_isHiding = false;
			_camera = Camera.main;
			SetEmote(text);
			SetPosition();
			Show();
		}

		protected virtual void SetEmote(string text)
		{
			_text.text = text;
		}

		protected void SetPosition()
		{
			if (_parent != null)
			{
				Vector3 position = _parent.position + offset;
				Vector3 position2 = _camera.WorldToScreenPoint(position);
				_transform.position = position2;
			}
		}

		protected void LateUpdate()
		{
			SetPosition();
			_currentShowTime -= Time.deltaTime;
			if (_currentShowTime <= 0f && !_isHiding)
			{
				_isHiding = true;
				Hide();
			}
		}
	}
}
