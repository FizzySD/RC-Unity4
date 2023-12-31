using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class SliderInputSettingElement : BaseSettingElement
	{
		protected Slider _slider;

		protected InputField _inputField;

		protected int _inputFontSizeOffset = -4;

		protected NumberFormatInfo _formatInfo;

		protected bool _fixedInputField;

		protected override HashSet<SettingType> SupportedSettingTypes
		{
			get
			{
				return new HashSet<SettingType>
				{
					SettingType.Float,
					SettingType.Int
				};
			}
		}

		public void Setup(BaseSetting setting, ElementStyle style, string title, string tooltip, float sliderWidth, float sliderHeight, float inputWidth, float inputHeight, int decimalPlaces)
		{
			_formatInfo = new NumberFormatInfo();
			_formatInfo.NumberDecimalDigits = decimalPlaces;
			_slider = base.transform.Find("Slider").GetComponent<Slider>();
			_settingType = GetSettingType(setting);
			if (_settingType == SettingType.Int)
			{
				_slider.wholeNumbers = true;
				_slider.minValue = ((IntSetting)setting).MinValue;
				_slider.maxValue = ((IntSetting)setting).MaxValue;
			}
			else if (_settingType == SettingType.Float)
			{
				_slider.wholeNumbers = false;
				_slider.minValue = ((FloatSetting)setting).MinValue;
				_slider.maxValue = ((FloatSetting)setting).MaxValue;
			}
			_slider.GetComponent<LayoutElement>().preferredWidth = sliderWidth;
			_slider.GetComponent<LayoutElement>().preferredHeight = sliderHeight;
			_slider.onValueChanged.AddListener(delegate(float value)
			{
				OnSliderValueChanged(value);
			});
			_inputField = base.transform.Find("InputField").GetComponent<InputField>();
			_inputField.transform.Find("Text").GetComponent<Text>().fontSize = style.FontSize + _inputFontSizeOffset;
			_inputField.GetComponent<LayoutElement>().preferredWidth = inputWidth;
			_inputField.GetComponent<LayoutElement>().preferredHeight = inputHeight;
			_settingType = GetSettingType(setting);
			if (_settingType == SettingType.Float)
			{
				_inputField.contentType = InputField.ContentType.DecimalNumber;
			}
			else if (_settingType == SettingType.Int)
			{
				_inputField.contentType = InputField.ContentType.IntegerNumber;
			}
			_inputField.onValueChange.AddListener(delegate(string value)
			{
				OnInputValueChanged(value);
			});
			_inputField.onEndEdit.AddListener(delegate(string value)
			{
				OnInputFinishEditing(value);
			});
			base.Setup(setting, style, title, tooltip);
			_slider.transform.Find("Background").GetComponent<Image>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "SliderBackgroundColor");
			_slider.transform.Find("Fill Area/Fill").GetComponent<Image>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "SliderFillColor");
			_slider.transform.Find("Handle Slide Area/Handle").GetComponent<Image>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "SliderHandleColor");
			_inputField.colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "Input");
			_inputField.transform.Find("Text").GetComponent<Text>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "InputTextColor");
			_inputField.selectionColor = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "InputSelectionColor");
			StartCoroutine(WaitAndFixInputField());
		}

		private void OnEnable()
		{
			if (_inputField != null && !_fixedInputField)
			{
				_inputField.gameObject.SetActive(false);
				_inputField.gameObject.SetActive(true);
			}
		}

		private IEnumerator WaitAndFixInputField()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			_inputField.gameObject.SetActive(false);
			_inputField.gameObject.SetActive(true);
			_fixedInputField = true;
		}

		protected void OnSliderValueChanged(float value)
		{
			if (_settingType == SettingType.Float)
			{
				((FloatSetting)_setting).Value = value;
			}
			else if (_settingType == SettingType.Int)
			{
				((IntSetting)_setting).Value = (int)value;
			}
			SyncInput();
		}

		protected void OnInputValueChanged(string value)
		{
			if (value == string.Empty)
			{
				return;
			}
			int result2;
			if (_settingType == SettingType.Float)
			{
				float result;
				if (float.TryParse(value, out result))
				{
					((FloatSetting)_setting).Value = Mathf.Clamp(result, _slider.minValue, _slider.maxValue);
				}
			}
			else if (_settingType == SettingType.Int && int.TryParse(value, out result2))
			{
				((IntSetting)_setting).Value = (int)Mathf.Clamp(result2, _slider.minValue, _slider.maxValue);
			}
		}

		protected void OnInputFinishEditing(string value)
		{
			SyncElement();
		}

		protected void SyncSlider()
		{
			if (_settingType == SettingType.Float)
			{
				_slider.value = ((FloatSetting)_setting).Value;
			}
			else if (_settingType == SettingType.Int)
			{
				_slider.value = ((IntSetting)_setting).Value;
			}
		}

		protected void SyncInput()
		{
			if (_settingType == SettingType.Float)
			{
				_inputField.text = string.Format(_formatInfo, "{0:N}", ((FloatSetting)_setting).Value);
			}
			else if (_settingType == SettingType.Int)
			{
				_inputField.text = ((IntSetting)_setting).Value.ToString();
			}
		}

		public override void SyncElement()
		{
			SyncSlider();
			SyncInput();
		}
	}
}
