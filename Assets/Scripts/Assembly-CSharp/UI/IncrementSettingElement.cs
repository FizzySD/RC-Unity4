using System.Collections.Generic;
using Settings;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	internal class IncrementSettingElement : BaseSettingElement
	{
		protected Text _valueLabel;

		protected string[] _options;

		protected UnityAction _onValueChanged;

		protected override HashSet<SettingType> SupportedSettingTypes
		{
			get
			{
				return new HashSet<SettingType> { SettingType.Int };
			}
		}

		public void Setup(BaseSetting setting, ElementStyle style, string title, string tooltip, float elementWidth, float elementHeight, string[] options, UnityAction onValueChanged)
		{
			_valueLabel = base.transform.Find("Increment/ValueLabel").GetComponent<Text>();
			_valueLabel.fontSize = style.FontSize;
			_options = options;
			_onValueChanged = onValueChanged;
			Button component = base.transform.Find("Increment/LeftButton").GetComponent<Button>();
			Button component2 = base.transform.Find("Increment/RightButton").GetComponent<Button>();
			LayoutElement component3 = component.GetComponent<LayoutElement>();
			LayoutElement component4 = component2.GetComponent<LayoutElement>();
			component.onClick.AddListener(delegate
			{
				OnButtonPressed(false);
			});
			component2.onClick.AddListener(delegate
			{
				OnButtonPressed(true);
			});
			float preferredWidth = (component4.preferredWidth = elementWidth);
			component3.preferredWidth = preferredWidth;
			preferredWidth = (component4.preferredHeight = elementHeight);
			component3.preferredHeight = preferredWidth;
			base.Setup(setting, style, title, tooltip);
			component.colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultButton", "");
			component2.colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultButton", "");
			_valueLabel.color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "TextColor");
		}

		protected void OnButtonPressed(bool increment)
		{
			if (_settingType == SettingType.Int)
			{
				if (increment)
				{
					((IntSetting)_setting).Value++;
				}
				else
				{
					((IntSetting)_setting).Value--;
				}
			}
			UpdateValueLabel();
			if (_onValueChanged != null)
			{
				_onValueChanged();
			}
		}

		protected void UpdateValueLabel()
		{
			if (_settingType == SettingType.Int)
			{
				if (_options == null)
				{
					_valueLabel.text = ((IntSetting)_setting).Value.ToString();
				}
				else
				{
					_valueLabel.text = _options[((IntSetting)_setting).Value];
				}
			}
		}

		public override void SyncElement()
		{
			UpdateValueLabel();
		}
	}
}
