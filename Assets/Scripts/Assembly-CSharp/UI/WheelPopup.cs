using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	internal class WheelPopup : BasePopup
	{
		private Text _centerText;

		private List<GameObject> _buttons = new List<GameObject>();

		public int SelectedItem;

		private UnityAction _callback;

		protected override float AnimationTime
		{
			get
			{
				return 0.2f;
			}
		}

		protected override PopupAnimation PopupAnimationType
		{
			get
			{
				return PopupAnimation.Fade;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			_centerText = base.transform.Find("Panel/Center/Label").GetComponent<Text>();
			for (int i = 0; i < 8; i++)
			{
				_buttons.Add(ElementFactory.InstantiateAndBind(base.transform.Find("Panel/Buttons"), "WheelButton").gameObject);
				int index = i;
				_buttons[index].GetComponent<Button>().onClick.AddListener(delegate
				{
					OnButtonClick(index);
				});
			}
			ElementFactory.SetAnchor(_buttons[0], TextAnchor.MiddleCenter, TextAnchor.LowerCenter, new Vector2(0f, 180f));
			ElementFactory.SetAnchor(_buttons[1], TextAnchor.MiddleCenter, TextAnchor.LowerLeft, new Vector2(135f, 90f));
			ElementFactory.SetAnchor(_buttons[2], TextAnchor.MiddleCenter, TextAnchor.MiddleLeft, new Vector2(180f, 0f));
			ElementFactory.SetAnchor(_buttons[3], TextAnchor.MiddleCenter, TextAnchor.UpperLeft, new Vector2(135f, -90f));
			ElementFactory.SetAnchor(_buttons[4], TextAnchor.MiddleCenter, TextAnchor.UpperCenter, new Vector2(0f, -180f));
			ElementFactory.SetAnchor(_buttons[5], TextAnchor.MiddleCenter, TextAnchor.UpperRight, new Vector2(-135f, -90f));
			ElementFactory.SetAnchor(_buttons[6], TextAnchor.MiddleCenter, TextAnchor.MiddleRight, new Vector2(-180f, 0f));
			ElementFactory.SetAnchor(_buttons[7], TextAnchor.MiddleCenter, TextAnchor.LowerRight, new Vector2(-135f, 90f));
		}

		public void Show(string openKey, List<string> options, UnityAction callback)
		{
			if (base.gameObject.activeSelf)
			{
				StopAllCoroutines();
				SetTransformAlpha(MaxFadeAlpha);
			}
			SetCenterText(openKey);
			_callback = callback;
			for (int i = 0; i < options.Count; i++)
			{
				_buttons[i].SetActive(true);
				KeybindSetting keybindSetting = (KeybindSetting)SettingsManager.InputSettings.Interaction.Settings["QuickSelect" + (i + 1)];
				_buttons[i].transform.Find("Text").GetComponent<Text>().text = keybindSetting.ToString() + " - " + options[i];
			}
			for (int j = options.Count; j < _buttons.Count; j++)
			{
				_buttons[j].SetActive(false);
			}
			Show();
		}

		private void SetCenterText(string openKey)
		{
			_centerText.text = SettingsManager.InputSettings.Interaction.MenuNext.ToString() + " - " + UIManager.GetLocaleCommon("Next") + "\n";
			Text centerText = _centerText;
			centerText.text = centerText.text + openKey + " - " + UIManager.GetLocaleCommon("Cancel");
		}

		private void OnButtonClick(int index)
		{
			SelectedItem = index;
			_callback();
		}

		private void Update()
		{
			for (int i = 0; i < 8; i++)
			{
				KeybindSetting keybindSetting = (KeybindSetting)SettingsManager.InputSettings.Interaction.Settings["QuickSelect" + (i + 1)];
				if (keybindSetting.GetKeyDown())
				{
					OnButtonClick(i);
				}
			}
		}
	}
}
