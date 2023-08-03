using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	public string keyName;

	private UICheckbox mCheck;

	private UIPopupList mList;

	private string key
	{
		get
		{
			if (string.IsNullOrEmpty(keyName))
			{
				return "NGUI State: " + base.name;
			}
			return keyName;
		}
	}

	private void Awake()
	{
		mList = GetComponent<UIPopupList>();
		mCheck = GetComponent<UICheckbox>();
		if (mList != null)
		{
			mList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Combine(mList.onSelectionChange, new UIPopupList.OnSelectionChange(SaveSelection));
		}
		if (mCheck != null)
		{
			mCheck.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(mCheck.onStateChange, new UICheckbox.OnStateChange(SaveState));
		}
	}

	private void OnDestroy()
	{
		if (mCheck != null)
		{
			mCheck.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(mCheck.onStateChange, new UICheckbox.OnStateChange(SaveState));
		}
		if (mList != null)
		{
			mList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Remove(mList.onSelectionChange, new UIPopupList.OnSelectionChange(SaveSelection));
		}
	}

	private void OnDisable()
	{
		if (!(mCheck == null) || !(mList == null))
		{
			return;
		}
		UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UICheckbox uICheckbox = componentsInChildren[i];
			if (uICheckbox.isChecked)
			{
				SaveSelection(uICheckbox.name);
				break;
			}
		}
	}

	private void OnEnable()
	{
		if (mList != null)
		{
			string @string = PlayerPrefs.GetString(key);
			if (!string.IsNullOrEmpty(@string))
			{
				mList.selection = @string;
			}
			return;
		}
		if (mCheck != null)
		{
			mCheck.isChecked = PlayerPrefs.GetInt(key, 1) != 0;
			return;
		}
		string string2 = PlayerPrefs.GetString(key);
		UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UICheckbox uICheckbox = componentsInChildren[i];
			uICheckbox.isChecked = uICheckbox.name == string2;
		}
	}

	private void SaveSelection(string selection)
	{
		PlayerPrefs.SetString(key, selection);
	}

	private void SaveState(bool state)
	{
		PlayerPrefs.SetInt(key, state ? 1 : 0);
	}
}
