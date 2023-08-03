using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class MessagePopup : PromptPopup
	{
		protected float LabelHeight = 60f;

		private Text _label;

		protected override string Title
		{
			get
			{
				return string.Empty;
			}
		}

		protected override float Width
		{
			get
			{
				return 300f;
			}
		}

		protected override float Height
		{
			get
			{
				return 240f;
			}
		}

		protected override int VerticalPadding
		{
			get
			{
				return 30;
			}
		}

		protected override int HorizontalPadding
		{
			get
			{
				return 30;
			}
		}

		protected override TextAnchor PanelAlignment
		{
			get
			{
				return TextAnchor.MiddleCenter;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			ElementStyle style = new ElementStyle(24, 120f, ThemePanel);
			ElementStyle style2 = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			ElementFactory.CreateDefaultButton(BottomBar, style2, UIManager.GetLocaleCommon("Okay"), 0f, 0f, delegate
			{
				OnButtonClick("Okay");
			});
			_label = ElementFactory.CreateDefaultLabel(SinglePanel, style, string.Empty).GetComponent<Text>();
			_label.GetComponent<LayoutElement>().preferredHeight = LabelHeight;
			_label.GetComponent<LayoutElement>().preferredWidth = Width - (float)(HorizontalPadding * 2);
		}

		public void Show(string message)
		{
			if (!base.gameObject.activeSelf)
			{
				Show();
				_label.text = message;
			}
		}

		private void OnButtonClick(string name)
		{
			Hide();
		}
	}
}
