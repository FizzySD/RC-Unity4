using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	internal class InputSettingElement : BaseSettingElement
	{
		protected InputField _inputField;

		protected int _inputFontSizeOffset = -4;

		protected bool _fixedInputField;

		protected UnityAction _onValueChanged;

		protected UnityAction _onEndEdit;

		protected Transform _caret;

		protected bool _finishedSetup;

		protected object[] _setupParams;

		protected override HashSet<SettingType> SupportedSettingTypes
		{
			get
			{
				return new HashSet<SettingType>
				{
					SettingType.Float,
					SettingType.Int,
					SettingType.String
				};
			}
		}

		public void Setup(BaseSetting setting, ElementStyle style, string title, string tooltip, float elementWidth, float elementHeight, bool multiLine, UnityAction onValueChanged, UnityAction onEndEdit)
		{
			_onValueChanged = onValueChanged;
			_onEndEdit = onEndEdit;
			_inputField = base.transform.Find("InputField").gameObject.GetComponent<InputField>();
			if (_inputField == null)
			{
				_inputField = base.transform.Find("InputField").gameObject.AddComponent<InputFieldPasteable>();
				_inputField.textComponent = _inputField.transform.Find("Text").GetComponent<Text>();
				_inputField.transition = Selectable.Transition.ColorTint;
				_inputField.targetGraphic = _inputField.GetComponent<Image>();
				_inputField.text = "Default";
				_inputField.text = string.Empty;
			}
			_inputField.colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "Input");
			_inputField.transform.Find("Text").GetComponent<Text>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "InputTextColor");
			_inputField.selectionColor = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "InputSelectionColor");
			_inputField.transform.Find("Text").GetComponent<Text>().fontSize = style.FontSize + _inputFontSizeOffset;
			_inputField.transform.Find("Text").GetComponent<Text>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "InputTextColor");
			_inputField.GetComponent<LayoutElement>().preferredWidth = elementWidth;
			_inputField.GetComponent<LayoutElement>().preferredHeight = elementHeight;
			_inputField.lineType = (multiLine ? InputField.LineType.MultiLineNewline : InputField.LineType.SingleLine);
			_settingType = GetSettingType(setting);
			if (_settingType == SettingType.Float)
			{
				_inputField.contentType = InputField.ContentType.DecimalNumber;
				_inputField.characterLimit = 20;
			}
			else if (_settingType == SettingType.Int)
			{
				_inputField.contentType = InputField.ContentType.IntegerNumber;
				_inputField.characterLimit = 10;
			}
			else if (_settingType == SettingType.String)
			{
				_inputField.contentType = InputField.ContentType.Standard;
				int maxLength = ((StringSetting)setting).MaxLength;
				if (maxLength == int.MaxValue)
				{
					_inputField.characterLimit = 0;
				}
				else
				{
					_inputField.characterLimit = maxLength;
				}
			}
			_inputField.onValueChange.AddListener(delegate(string value)
			{
				OnValueChanged(value);
			});
			_inputField.onEndEdit.AddListener(delegate(string value)
			{
				OnInputFinishEditing(value);
			});
			if (multiLine)
			{
				_setupParams = new object[4] { setting, style, title, tooltip };
				StartCoroutine(WaitAndFinishSetup());
			}
			else
			{
				base.Setup(setting, style, title, tooltip);
				StartCoroutine(WaitAndFixInputField());
				_finishedSetup = true;
			}
		}

		private void OnEnable()
		{
			if (_inputField != null && !_finishedSetup)
			{
				StartCoroutine(WaitAndFinishSetup());
			}
			else if (_inputField != null && !_fixedInputField)
			{
				StartCoroutine(WaitAndFixInputField());
			}
		}

		private IEnumerator WaitAndFinishSetup()
		{
			yield return new WaitForEndOfFrame();
			_003C_003En__0((BaseSetting)_setupParams[0], (ElementStyle)_setupParams[1], (string)_setupParams[2], (string)_setupParams[2]);
			StartCoroutine(WaitAndFixInputField());
			_finishedSetup = true;
		}

		private IEnumerator WaitAndFixInputField()
		{
			yield return new WaitForEndOfFrame();
			_inputField.gameObject.SetActive(false);
			_inputField.gameObject.SetActive(true);
			SyncElement();
			_fixedInputField = true;
		}

		protected void OnValueChanged(string value)
		{
			if (!_finishedSetup)
			{
				return;
			}
			if (_settingType == SettingType.String)
			{
				((StringSetting)_setting).Value = _inputField.text;
			}
			else if (value != string.Empty)
			{
				int result2;
				if (_settingType == SettingType.Float)
				{
					float result;
					if (float.TryParse(value, out result))
					{
						((FloatSetting)_setting).Value = result;
					}
				}
				else if (_settingType == SettingType.Int && int.TryParse(value, out result2))
				{
					((IntSetting)_setting).Value = result2;
				}
			}
			if (_onValueChanged != null)
			{
				_onValueChanged();
			}
		}

		protected void OnInputFinishEditing(string value)
		{
			if (_finishedSetup)
			{
				SyncElement();
				if (_onEndEdit != null)
				{
					_onEndEdit();
				}
			}
		}

		public override void SyncElement()
		{
			if (_finishedSetup)
			{
				if (_settingType == SettingType.Float)
				{
					_inputField.text = ((FloatSetting)_setting).Value.ToString();
				}
				else if (_settingType == SettingType.Int)
				{
					_inputField.text = ((IntSetting)_setting).Value.ToString();
				}
				else if (_settingType == SettingType.String)
				{
					_inputField.text = ((StringSetting)_setting).Value;
				}
				_inputField.transform.Find("Text").GetComponent<Text>().text = _inputField.text;
			}
		}

		private void Update()
		{
			if ((bool)_caret || !(_inputField != null))
			{
				return;
			}
			_caret = _inputField.transform.Find(_inputField.transform.name + " Input Caret");
			if ((bool)_caret)
			{
				Graphic component = _caret.GetComponent<Graphic>();
				if (!component)
				{
					_caret.gameObject.AddComponent<Image>();
				}
			}
		}

		[CompilerGenerated]
		[DebuggerHidden]
		private void _003C_003En__0(BaseSetting setting, ElementStyle style, string title, string tooltip)
		{
			base.Setup(setting, style, title, tooltip);
		}
	}
}
