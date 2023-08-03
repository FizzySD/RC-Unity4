using Settings;

namespace UI
{
	internal class ExportPopup : PromptPopup
	{
		private InputSettingElement _element;

		public StringSetting ExportSetting = new StringSetting(string.Empty);

		protected override string Title
		{
			get
			{
				return UIManager.GetLocaleCommon("Export");
			}
		}

		protected override float Width
		{
			get
			{
				return 500f;
			}
		}

		protected override float Height
		{
			get
			{
				return 600f;
			}
		}

		protected override int VerticalPadding
		{
			get
			{
				return 20;
			}
		}

		protected override int HorizontalPadding
		{
			get
			{
				return 20;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Done"), 0f, 0f, delegate
			{
				OnButtonClick("Done");
			});
			_element = ElementFactory.CreateInputSetting(SinglePanel, style, ExportSetting, string.Empty, "", 460f, 440f, true).GetComponent<InputSettingElement>();
		}

		public void Show(string value)
		{
			if (!base.gameObject.activeSelf)
			{
				Show();
				ExportSetting.Value = value;
				_element.SyncElement();
			}
		}

		private void OnButtonClick(string name)
		{
			if (name == "Done")
			{
				Hide();
			}
		}
	}
}
