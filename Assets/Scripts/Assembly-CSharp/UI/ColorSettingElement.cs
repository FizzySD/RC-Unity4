using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class ColorSettingElement : BaseSettingElement
	{
		private Image _image;

		private ColorPickPopup _colorPickPopup;

		protected override HashSet<SettingType> SupportedSettingTypes
		{
			get
			{
				return new HashSet<SettingType> { SettingType.Color };
			}
		}

		public void Setup(BaseSetting setting, ElementStyle style, string title, ColorPickPopup colorPickPopup, string tooltip, float elementWidth, float elementHeight)
		{
			_colorPickPopup = colorPickPopup;
			GameObject gameObject = base.transform.Find("ColorButton").gameObject;
			gameObject.GetComponent<LayoutElement>().preferredWidth = elementWidth;
			gameObject.GetComponent<LayoutElement>().preferredHeight = elementHeight;
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnButtonClicked();
			});
			gameObject.GetComponent<Button>().colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "Icon");
			_image = gameObject.transform.Find("Border/Image").GetComponent<Image>();
			base.Setup(setting, style, title, tooltip);
		}

		protected void OnButtonClicked()
		{
			_colorPickPopup.Show((ColorSetting)_setting, _image);
		}

		public override void SyncElement()
		{
			_image.color = ((ColorSetting)_setting).Value;
		}
	}
}
