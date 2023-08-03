using UnityEngine.UI;

namespace UI
{
	internal class TooltipButton : Button
	{
		private string _tooltipMessage;

		private new void Awake()
		{
			base.transition = Transition.ColorTint;
			base.targetGraphic = GetComponent<Graphic>();
		}

		public virtual void Setup(string tooltipMessage, ElementStyle style)
		{
			_tooltipMessage = tooltipMessage;
			base.colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "Icon");
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			if (UIManager.CurrentMenu == null)
			{
				return;
			}
			TooltipPopup tooltipPopup = UIManager.CurrentMenu.TooltipPopup;
			switch (state)
			{
			case SelectionState.Highlighted:
			case SelectionState.Pressed:
				tooltipPopup.Show(_tooltipMessage, this);
				break;
			case SelectionState.Normal:
				if (tooltipPopup.Caller == this)
				{
					UIManager.CurrentMenu.TooltipPopup.Hide();
				}
				break;
			}
		}
	}
}
