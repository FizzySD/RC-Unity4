using UnityEngine.UI;

namespace UI
{
	internal class IntroButton : Button
	{
		private float _fadeTime = 0.1f;

		private Image _hoverImage;

		protected override void Awake()
		{
			_hoverImage = base.transform.Find("HoverImage").GetComponent<Image>();
			_hoverImage.canvasRenderer.SetAlpha(0f);
			base.transition = Transition.ColorTint;
			base.targetGraphic = base.transform.Find("Content/Label").GetComponent<Graphic>();
			if (base.gameObject.name.StartsWith("Settings") || base.gameObject.name.StartsWith("Quit") || base.gameObject.name.StartsWith("Profile"))
			{
				base.targetGraphic.GetComponent<Text>().text = UIManager.GetLocaleCommon(base.gameObject.name.Replace("Button", string.Empty));
			}
			else
			{
				base.targetGraphic.GetComponent<Text>().text = UIManager.GetLocale("MainMenu", "Intro", base.gameObject.name);
			}
			ColorBlock colorBlock = default(ColorBlock);
			ColorBlock themeColorBlock = UIManager.GetThemeColorBlock("MainMenu", "IntroButton", "");
			colorBlock.normalColor = themeColorBlock.normalColor;
			colorBlock.highlightedColor = themeColorBlock.highlightedColor;
			colorBlock.pressedColor = themeColorBlock.pressedColor;
			colorBlock.colorMultiplier = 1f;
			colorBlock.fadeDuration = _fadeTime;
			base.colors = colorBlock;
			base.navigation = new Navigation
			{
				mode = Navigation.Mode.None
			};
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			Image component = base.transform.Find("Content/Icon").GetComponent<Image>();
			switch (state)
			{
			case SelectionState.Highlighted:
			case SelectionState.Pressed:
				_hoverImage.CrossFadeAlpha(1f, _fadeTime, true);
				switch (state)
				{
				case SelectionState.Pressed:
					component.CrossFadeColor(UIManager.GetThemeColor("MainMenu", "IntroButton", "PressedColor"), _fadeTime, true, true);
					break;
				case SelectionState.Highlighted:
					component.CrossFadeColor(UIManager.GetThemeColor("MainMenu", "IntroButton", "HighlightedColor"), _fadeTime, true, true);
					break;
				}
				break;
			case SelectionState.Normal:
				_hoverImage.CrossFadeAlpha(0f, _fadeTime, true);
				component.CrossFadeColor(UIManager.GetThemeColor("MainMenu", "IntroButton", "NormalColor"), _fadeTime, true, true);
				break;
			}
		}
	}
}
