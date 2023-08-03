using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class ColorPickPopup : PromptPopup
	{
		private float PreviewWidth = 90f;

		private float PreviewHeight = 40f;

		private Image _image;

		private ColorSetting _setting;

		private Image _preview;

		private FloatSetting _red = new FloatSetting(0f, 0f, 1f);

		private FloatSetting _green = new FloatSetting(0f, 0f, 1f);

		private FloatSetting _blue = new FloatSetting(0f, 0f, 1f);

		private FloatSetting _alpha = new FloatSetting(0f, 0f, 1f);

		private List<GameObject> _sliders = new List<GameObject>();

		protected override string Title
		{
			get
			{
				return UIManager.GetLocale("SettingsPopup", "ColorPickPopup", "Title");
			}
		}

		protected override float Width
		{
			get
			{
				return 450f;
			}
		}

		protected override float Height
		{
			get
			{
				return 450f;
			}
		}

		protected override float VerticalSpacing
		{
			get
			{
				return 20f;
			}
		}

		protected override TextAnchor PanelAlignment
		{
			get
			{
				return TextAnchor.UpperCenter;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Save"), 0f, 0f, delegate
			{
				OnButtonClick("Save");
			});
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Cancel"), 0f, 0f, delegate
			{
				OnButtonClick("Cancel");
			});
			GameObject gameObject = ElementFactory.InstantiateAndBind(SinglePanel, "ColorPreview");
			gameObject.GetComponent<LayoutElement>().preferredWidth = PreviewWidth;
			gameObject.GetComponent<LayoutElement>().preferredHeight = PreviewHeight;
			_preview = gameObject.transform.Find("Image").GetComponent<Image>();
		}

		private void Update()
		{
			if (_preview != null)
			{
				_preview.color = GetColorFromSliders();
			}
		}

		public void Show(ColorSetting setting, Image image)
		{
			if (!base.gameObject.activeSelf)
			{
				Show();
				_setting = setting;
				_image = image;
				_red.Value = setting.Value.r;
				_green.Value = setting.Value.g;
				_blue.Value = setting.Value.b;
				_alpha.MinValue = setting.MinAlpha;
				_alpha.Value = setting.Value.a;
				_preview.color = GetColorFromSliders();
				CreateSliders();
			}
		}

		private void CreateSliders()
		{
			foreach (GameObject slider in _sliders)
			{
				Object.Destroy(slider);
			}
			ElementStyle style = new ElementStyle(24, 85f, ThemePanel);
			_sliders.Add(ElementFactory.CreateSliderInputSetting(SinglePanel, style, _red, "Red", "", 150f, 16f, 70f, 40f, 3));
			_sliders.Add(ElementFactory.CreateSliderInputSetting(SinglePanel, style, _green, "Green", "", 150f, 16f, 70f, 40f, 3));
			_sliders.Add(ElementFactory.CreateSliderInputSetting(SinglePanel, style, _blue, "Blue", "", 150f, 16f, 70f, 40f, 3));
			_sliders.Add(ElementFactory.CreateSliderInputSetting(SinglePanel, style, _alpha, "Alpha", "", 150f, 16f, 70f, 40f, 3));
		}

		private void OnButtonClick(string name)
		{
			if (name == "Cancel")
			{
				Hide();
			}
			else if (name == "Save")
			{
				_setting.Value = GetColorFromSliders();
				_image.color = _setting.Value;
				Hide();
			}
		}

		private Color GetColorFromSliders()
		{
			return new Color(_red.Value, _green.Value, _blue.Value, Mathf.Clamp(_alpha.Value, _setting.MinAlpha, 1f));
		}
	}
}
