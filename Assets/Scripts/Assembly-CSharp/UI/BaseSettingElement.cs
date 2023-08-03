using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal abstract class BaseSettingElement : MonoBehaviour
	{
		protected BaseSetting _setting;

		protected SettingType _settingType;

		protected ElementStyle _style;

		protected virtual HashSet<SettingType> SupportedSettingTypes
		{
			get
			{
				return new HashSet<SettingType>();
			}
		}

		public virtual void Setup(BaseSetting setting, ElementStyle style, string title, string tooltip)
		{
			_setting = setting;
			_settingType = GetSettingType(setting);
			_style = style;
			if (!SupportedSettingTypes.Contains(_settingType))
			{
				throw new ArgumentException("Unsupported setting type being used for UI element.");
			}
			SetupTitle(title, style.FontSize, style.TitleWidth);
			SetupTooltip(tooltip, style);
			SyncElement();
		}

		protected void SetupTooltip(string tooltip, ElementStyle style)
		{
			GameObject gameObject = base.transform.Find("TooltipIcon").gameObject;
			if (tooltip == string.Empty)
			{
				gameObject.SetActive(false);
				return;
			}
			TooltipButton tooltipButton = gameObject.AddComponent<TooltipButton>();
			tooltipButton.Setup(tooltip, style);
		}

		public abstract void SyncElement();

		protected SettingType GetSettingType(BaseSetting setting)
		{
			Type type = setting.GetType();
			if (type == typeof(IntSetting))
			{
				return SettingType.Int;
			}
			if (type == typeof(FloatSetting))
			{
				return SettingType.Float;
			}
			if (type == typeof(StringSetting) || type == typeof(NameSetting))
			{
				return SettingType.String;
			}
			if (type == typeof(BoolSetting))
			{
				return SettingType.Bool;
			}
			if (type == typeof(KeybindSetting))
			{
				return SettingType.Keybind;
			}
			if (type == typeof(ColorSetting))
			{
				return SettingType.Color;
			}
			throw new ArgumentException("Invalid setting type found.");
		}

		protected void SetupTitle(string title, int fontSize, float titleWidth)
		{
			GameObject gameObject = base.gameObject.transform.Find("Label").gameObject;
			if (title == string.Empty)
			{
				gameObject.SetActive(false);
				return;
			}
			SetupLabel(gameObject, title, fontSize);
			gameObject.GetComponent<LayoutElement>().preferredWidth = titleWidth;
			if (titleWidth <= 0f)
			{
				gameObject.GetComponent<LayoutElement>().preferredWidth = -1f;
			}
			gameObject.GetComponent<Text>().color = UIManager.GetThemeColor(_style.ThemePanel, "DefaultSetting", "TextColor");
		}

		protected void SetupLabel(GameObject obj, string title, int fontSize)
		{
			Text component = obj.GetComponent<Text>();
			component.text = title;
			component.fontSize = fontSize;
		}

		protected void SetupLabel(GameObject obj, string title)
		{
			Text component = obj.GetComponent<Text>();
			component.text = title;
		}
	}
}
