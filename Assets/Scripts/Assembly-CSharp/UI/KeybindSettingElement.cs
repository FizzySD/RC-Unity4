using System.Collections.Generic;
using ApplicationManagers;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class KeybindSettingElement : BaseSettingElement
	{
		private List<Text> _buttonLabels = new List<Text>();

		private KeybindPopup _keybindPopup;

		protected override HashSet<SettingType> SupportedSettingTypes
		{
			get
			{
				return new HashSet<SettingType> { SettingType.Keybind };
			}
		}

		public void Setup(BaseSetting setting, ElementStyle style, string title, KeybindPopup keybindPopup, string tooltip, float elementWidth, float elementHeight, int bindCount)
		{
			_keybindPopup = keybindPopup;
			for (int i = 0; i < bindCount; i++)
			{
				CreateKeybindButton(i, style, elementWidth, elementHeight);
			}
			base.Setup(setting, style, title, tooltip);
		}

		private void CreateKeybindButton(int index, ElementStyle style, float width, float height)
		{
			GameObject gameObject = AssetBundleManager.InstantiateAsset<GameObject>("KeybindButton");
			Text component = gameObject.transform.Find("Text").GetComponent<Text>();
			component.color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "KeybindTextColor");
			component.fontSize = style.FontSize;
			gameObject.GetComponent<LayoutElement>().preferredWidth = width;
			gameObject.GetComponent<LayoutElement>().preferredHeight = height;
			gameObject.transform.SetParent(base.transform, false);
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnButtonClicked(index);
			});
			gameObject.GetComponent<Button>().colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "Keybind");
			_buttonLabels.Add(component);
		}

		protected void OnButtonClicked(int index)
		{
			_keybindPopup.Show(((KeybindSetting)_setting).InputKeys[index], _buttonLabels[index]);
		}

		public override void SyncElement()
		{
			for (int i = 0; i < _buttonLabels.Count; i++)
			{
				_buttonLabels[i].text = ((KeybindSetting)_setting).InputKeys[i].ToString();
			}
		}
	}
}
