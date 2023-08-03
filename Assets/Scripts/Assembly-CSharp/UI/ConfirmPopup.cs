using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	internal class ConfirmPopup : PromptPopup
	{
		protected float LabelHeight = 60f;

		private Text _label;

		private UnityAction _onConfirm;

		protected override string Title
		{
			get
			{
				return UIManager.GetLocaleCommon("Confirm");
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
			ElementFactory.CreateDefaultButton(BottomBar, style2, UIManager.GetLocaleCommon("Confirm"), 0f, 0f, delegate
			{
				OnButtonClick("Confirm");
			});
			ElementFactory.CreateDefaultButton(BottomBar, style2, UIManager.GetLocaleCommon("Cancel"), 0f, 0f, delegate
			{
				OnButtonClick("Cancel");
			});
			_label = ElementFactory.CreateDefaultLabel(SinglePanel, style, string.Empty).GetComponent<Text>();
			_label.GetComponent<LayoutElement>().preferredWidth = Width - (float)(HorizontalPadding * 2);
			_label.GetComponent<LayoutElement>().preferredHeight = LabelHeight;
		}

		public void Show(string message, UnityAction onConfirm, string title = null)
		{
			if (!base.gameObject.activeSelf)
			{
				Show();
				_label.text = message;
				_onConfirm = onConfirm;
				if (title != null)
				{
					SetTitle(title);
				}
				else
				{
					SetTitle(Title);
				}
			}
		}

		private void OnButtonClick(string name)
		{
			if (name == "Confirm")
			{
				_onConfirm();
			}
			Hide();
		}
	}
}
