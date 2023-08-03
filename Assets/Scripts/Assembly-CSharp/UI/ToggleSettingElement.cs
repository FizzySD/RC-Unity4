using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class ToggleSettingElement : BaseSettingElement
	{
		protected Toggle _toggle;

		private float _checkMarkSizeMultiplier = 0.66f;

		protected override HashSet<SettingType> SupportedSettingTypes
		{
			get
			{
				return new HashSet<SettingType> { SettingType.Bool };
			}
		}

		public void Setup(BaseSetting setting, ElementStyle style, string title, string tooltip, float elementWidth, float elementHeight)
		{
			_toggle = base.transform.Find("Toggle").GetComponent<Toggle>();
			_toggle.onValueChanged.AddListener(delegate(bool value)
			{
				OnValueChanged(value);
			});
			LayoutElement component = _toggle.transform.Find("Background").GetComponent<LayoutElement>();
			RectTransform component2 = component.transform.Find("Checkmark").GetComponent<RectTransform>();
			component.preferredHeight = elementHeight;
			component.preferredWidth = elementWidth;
			component2.sizeDelta = new Vector2(elementWidth * _checkMarkSizeMultiplier, elementHeight * _checkMarkSizeMultiplier);
			base.Setup(setting, style, title, tooltip);
			component2.GetComponent<Image>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "ToggleFilledColor");
			_toggle.colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "Toggle");
		}

		protected void OnValueChanged(bool value)
		{
			((BoolSetting)_setting).Value = value;
		}

		public override void SyncElement()
		{
			_toggle.isOn = ((BoolSetting)_setting).Value;
		}
	}
}
