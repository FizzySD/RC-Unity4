using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class TooltipPopup : BasePopup
	{
		private Text _label;

		private RectTransform _panel;

		public TooltipButton Caller;

		protected override float AnimationTime
		{
			get
			{
				return 0.15f;
			}
		}

		protected override PopupAnimation PopupAnimationType
		{
			get
			{
				return PopupAnimation.Fade;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			_label = base.transform.Find("Panel/Label").GetComponent<Text>();
			_label.text = string.Empty;
			_panel = base.transform.Find("Panel").GetComponent<RectTransform>();
			_label.color = UIManager.GetThemeColor(ThemePanel, "DefaultSetting", "TooltipTextColor");
			_panel.Find("Background").GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "DefaultSetting", "TooltipBackgroundColor");
		}

		public void Show(string message, TooltipButton caller)
		{
			if (base.gameObject.activeSelf)
			{
				StopAllCoroutines();
				SetTransformAlpha(MaxFadeAlpha);
			}
			_label.text = message;
			Caller = caller;
			Canvas.ForceUpdateCanvases();
			SetTooltipPosition();
			Show();
		}

		private void SetTooltipPosition()
		{
			float x = GetComponent<RectTransform>().sizeDelta.x;
			float num = (x * 0.5f + 40f) * UIManager.CurrentCanvasScale;
			Vector3 position = Caller.transform.position;
			if (position.x + num > (float)Screen.width)
			{
				position.x -= num;
			}
			else
			{
				position.x += num;
			}
			base.transform.position = position;
		}

		private void Update()
		{
			if (Caller != null)
			{
				SetTooltipPosition();
			}
		}
	}
}
