using System;
using System.Collections;
using System.Collections.Generic;
using ApplicationManagers;
using Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	internal class DropdownSettingElement : BaseSettingElement
	{
		protected GameObject _optionsPanel;

		protected GameObject _selectedButton;

		protected GameObject _selectedButtonLabel;

		protected string[] _options;

		protected float _currentScrollValue = 1f;

		protected Scrollbar _scrollBar;

		private Vector3 _optionsOffset;

		private UnityAction _onDropdownOptionSelect;

		private Vector3 _lastKnownPosition = Vector3.zero;

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

		public void Setup(BaseSetting setting, ElementStyle style, string title, string[] options, string tooltip, float elementWidth, float elementHeight, float optionsWidth, float maxScrollHeight, UnityAction onDropdownOptionSelect)
		{
			if (options.Length == 0)
			{
				throw new ArgumentException("Dropdown cannot have 0 options.");
			}
			_onDropdownOptionSelect = onDropdownOptionSelect;
			_options = options;
			_optionsPanel = base.transform.Find("Dropdown/Mask").gameObject;
			_selectedButton = base.transform.Find("Dropdown/SelectedButton").gameObject;
			_selectedButtonLabel = _selectedButton.transform.Find("Label").gameObject;
			SetupLabel(_selectedButtonLabel, options[0], style.FontSize);
			_selectedButton.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnDropdownSelectedButtonClick();
			});
			_selectedButton.GetComponent<LayoutElement>().preferredWidth = elementWidth;
			_selectedButton.GetComponent<LayoutElement>().preferredHeight = elementHeight;
			for (int i = 0; i < options.Length; i++)
			{
				CreateOptionButton(options[i], i, optionsWidth, elementHeight, style.FontSize, style.ThemePanel);
			}
			_selectedButton.GetComponent<Button>().colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "Dropdown");
			_selectedButtonLabel.GetComponent<Text>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "DropdownTextColor");
			_selectedButton.transform.Find("Image").GetComponent<Image>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "DropdownTextColor");
			_optionsPanel.transform.Find("Options").GetComponent<Image>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "DropdownBorderColor");
			Canvas.ForceUpdateCanvases();
			float num = _optionsPanel.transform.Find("Options").GetComponent<RectTransform>().sizeDelta.y;
			if (num > maxScrollHeight)
			{
				num = maxScrollHeight;
			}
			else
			{
				_optionsPanel.transform.Find("Scrollbar").gameObject.SetActive(false);
			}
			_scrollBar = _optionsPanel.transform.Find("Scrollbar").GetComponent<Scrollbar>();
			_scrollBar.colors = UIManager.GetThemeColorBlock(style.ThemePanel, "DefaultSetting", "DropdownScrollbar");
			_scrollBar.GetComponent<Image>().color = UIManager.GetThemeColor(style.ThemePanel, "DefaultSetting", "DropdownScrollbarBackgroundColor");
			_optionsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(optionsWidth, num);
			base.transform.Find("Label").GetComponent<LayoutElement>().preferredHeight = elementHeight;
			float x = (optionsWidth - elementWidth) * 0.5f;
			float y = (0f - (elementHeight + num)) * 0.5f + 2f;
			_optionsOffset = new Vector3(x, y, 0f);
			_optionsPanel.transform.SetParent(base.transform.root, true);
			_optionsPanel.SetActive(false);
			base.Setup(setting, style, title, tooltip);
		}

		protected void SetOptionsPosition()
		{
			Vector3 position = _selectedButton.transform.position + _optionsOffset * UIManager.CurrentCanvasScale;
			_optionsPanel.transform.GetComponent<RectTransform>().position = position;
		}

		private void OnDisable()
		{
			if (_optionsPanel != null)
			{
				_optionsPanel.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			if (_optionsPanel != null)
			{
				UnityEngine.Object.Destroy(_optionsPanel);
			}
		}

		private void Update()
		{
			if (_optionsPanel != null && _optionsPanel.activeSelf && ((Input.GetKeyUp(KeyCode.Mouse0) && EventSystem.current.currentSelectedGameObject != _scrollBar.gameObject) || base.transform.position != _lastKnownPosition))
			{
				StartCoroutine(WaitAndCloseOptions());
			}
		}

		protected void CreateOptionButton(string option, int index, float width, float height, int fontSize, string themePanel)
		{
			GameObject gameObject = AssetBundleManager.InstantiateAsset<GameObject>("DropdownOption");
			gameObject.transform.SetParent(_optionsPanel.transform.Find("Options"), false);
			SetupLabel(gameObject.transform.Find("Label").gameObject, option, fontSize);
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnDropdownOptionClick(option, index);
			});
			gameObject.GetComponent<LayoutElement>().preferredWidth = width;
			gameObject.GetComponent<LayoutElement>().preferredHeight = height;
			gameObject.GetComponent<Button>().colors = UIManager.GetThemeColorBlock(themePanel, "DefaultSetting", "Dropdown");
		}

		protected void OnDropdownSelectedButtonClick()
		{
			if (!_optionsPanel.activeSelf)
			{
				StartCoroutine(WaitAndEnableOptions());
			}
			else
			{
				CloseOptions();
			}
		}

		private IEnumerator WaitAndEnableOptions()
		{
			yield return new WaitForEndOfFrame();
			SetOptionsPosition();
			_optionsPanel.transform.SetAsLastSibling();
			_lastKnownPosition = base.transform.position;
			_optionsPanel.SetActive(true);
			yield return new WaitForEndOfFrame();
			_scrollBar.value = _currentScrollValue;
		}

		private IEnumerator WaitAndCloseOptions()
		{
			yield return new WaitForEndOfFrame();
			CloseOptions();
		}

		protected void OnDropdownOptionClick(string option, int index)
		{
			SetupLabel(_selectedButtonLabel, option);
			CloseOptions();
			if (_settingType == SettingType.String)
			{
				((StringSetting)_setting).Value = option;
			}
			else if (_settingType == SettingType.Int)
			{
				((IntSetting)_setting).Value = index;
			}
			UnityAction onDropdownOptionSelect = _onDropdownOptionSelect;
			if (onDropdownOptionSelect != null)
			{
				onDropdownOptionSelect();
			}
		}

		protected void CloseOptions()
		{
			_currentScrollValue = _scrollBar.value;
			_optionsPanel.SetActive(false);
		}

		public override void SyncElement()
		{
			if (_settingType == SettingType.String)
			{
				SetupLabel(_selectedButtonLabel, ((StringSetting)_setting).Value);
			}
			else if (_settingType == SettingType.Int)
			{
				IntSetting intSetting = (IntSetting)_setting;
				if (intSetting.Value >= _options.Length)
				{
					intSetting.Value = 0;
				}
				SetupLabel(_selectedButtonLabel, _options[intSetting.Value]);
			}
		}
	}
}
