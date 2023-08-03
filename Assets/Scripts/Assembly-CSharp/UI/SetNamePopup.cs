using Settings;
using UnityEngine.Events;

namespace UI
{
	internal class SetNamePopup : PromptPopup
	{
		private UnityAction _onSave;

		private InputSettingElement _element;

		public StringSetting NameSetting = new StringSetting(string.Empty);

		private string _initialValue;

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
				return 320f;
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
				return 40;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			ElementStyle style = new ElementStyle(ButtonFontSize, 100f, ThemePanel);
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Save"), 0f, 0f, delegate
			{
				OnButtonClick("Save");
			});
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Cancel"), 0f, 0f, delegate
			{
				OnButtonClick("Cancel");
			});
			_element = ElementFactory.CreateInputSetting(SinglePanel, style, NameSetting, UIManager.GetLocaleCommon("SetName"), "", 120f).GetComponent<InputSettingElement>();
		}

		public void Show(string initialValue, UnityAction onSave, string title)
		{
			if (!base.gameObject.activeSelf)
			{
				Show();
				_initialValue = initialValue;
				_onSave = onSave;
				SetTitle(title);
				NameSetting.Value = initialValue;
				_element.SyncElement();
			}
		}

		private void OnButtonClick(string name)
		{
			if (name == "Cancel")
			{
				Hide();
			}
			else if (name == "Save")
			{
				if (NameSetting.Value == string.Empty)
				{
					NameSetting.Value = _initialValue;
				}
				_onSave();
				Hide();
			}
		}
	}
}
