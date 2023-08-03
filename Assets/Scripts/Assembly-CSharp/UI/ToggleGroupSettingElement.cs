using System;
using System.Collections.Generic;
using ApplicationManagers;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class ToggleGroupSettingElement : BaseSettingElement
	{
		protected ToggleGroup _toggleGroup;

		protected GameObject _optionsPanel;

		protected string[] _options;

		protected List<Toggle> _toggles = new List<Toggle>();

		private float _checkMarkSizeMultiplier = 0.67f;

		protected override HashSet<SettingType> SupportedSettingTypes
		{
			get
			{
				return new HashSet<SettingType>
				{
					SettingType.String,
					SettingType.Int
				};
			}
		}

		public void Setup(BaseSetting setting, ElementStyle style, string title, string[] options, string tooltip, float elementWidth, float elementHeight)
		{
			if (options.Length == 0)
			{
				throw new ArgumentException("ToggleGroup cannot have 0 options.");
			}
			_options = options;
			_optionsPanel = base.transform.Find("Options").gameObject;
			_toggleGroup = _optionsPanel.GetComponent<ToggleGroup>();
			for (int i = 0; i < options.Length; i++)
			{
				_toggles.Add(CreateOptionToggle(options[i], i, style, elementWidth, elementHeight));
			}
			base.gameObject.transform.Find("Label").GetComponent<LayoutElement>().preferredHeight = elementHeight;
			base.Setup(setting, style, title, tooltip);
		}

		protected Toggle CreateOptionToggle(string option, int index, ElementStyle style, float width, float height)
		{
			GameObject gameObject = AssetBundleManager.InstantiateAsset<GameObject>("ToggleGroupOption");
			gameObject.transform.SetParent(_optionsPanel.transform, false);
			gameObject.transform.Find("Label").GetComponent<Text>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "TextColor");
			SetupLabel(gameObject.transform.Find("Label").gameObject, option, style.FontSize);
			LayoutElement component = gameObject.transform.Find("Background").GetComponent<LayoutElement>();
			RectTransform component2 = component.transform.Find("Checkmark").GetComponent<RectTransform>();
			component.preferredWidth = width;
			component.preferredHeight = height;
			component2.sizeDelta = new Vector2(width * _checkMarkSizeMultiplier, height * _checkMarkSizeMultiplier);
			Toggle component3 = gameObject.GetComponent<Toggle>();
			component3.group = _toggleGroup;
			component3.isOn = false;
			component3.onValueChanged.AddListener(delegate(bool value)
			{
				OnValueChanged(option, index, value);
			});
			component3.colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "Toggle");
			component2.GetComponent<Image>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "ToggleFilledColor");
			return component3;
		}

		protected void OnValueChanged(string option, int index, bool value)
		{
			if (value)
			{
				if (_settingType == SettingType.String)
				{
					((StringSetting)_setting).Value = option;
				}
				else if (_settingType == SettingType.Int)
				{
					((IntSetting)_setting).Value = index;
				}
			}
		}

		public override void SyncElement()
		{
			_toggleGroup.SetAllTogglesOff();
			if (_settingType == SettingType.String)
			{
				int index = FindOptionIndex(((StringSetting)_setting).Value);
				_toggles[index].isOn = true;
			}
			else if (_settingType == SettingType.Int)
			{
				_toggles[((IntSetting)_setting).Value].isOn = true;
			}
		}

		private int FindOptionIndex(string option)
		{
			for (int i = 0; i < _options.Length; i++)
			{
				if (_options[i] == option)
				{
					return i;
				}
			}
			throw new ArgumentOutOfRangeException("Option not found");
		}
	}
}
