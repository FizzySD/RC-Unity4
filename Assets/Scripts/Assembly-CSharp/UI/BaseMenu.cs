using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal abstract class BaseMenu : MonoBehaviour
	{
		protected List<BasePopup> _popups = new List<BasePopup>();

		public TooltipPopup TooltipPopup;

		public MessagePopup MessagePopup;

		public ConfirmPopup ConfirmPopup;

		public virtual void Setup()
		{
			SetupPopups();
		}

		public void ApplyScale()
		{
			StartCoroutine(WaitAndApplyScale());
		}

		protected IEnumerator WaitAndApplyScale()
		{
			float num = 1f / SettingsManager.UISettings.UIMasterScale.Value;
			GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f * num, 1080f * num);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			UIManager.CurrentCanvasScale = GetComponent<RectTransform>().localScale.x;
			BaseScaler[] componentsInChildren = GetComponentsInChildren<BaseScaler>(true);
			foreach (BaseScaler baseScaler in componentsInChildren)
			{
				baseScaler.ApplyScale();
			}
		}

		protected virtual void SetupPopups()
		{
			TooltipPopup = ElementFactory.CreateTooltipPopup<TooltipPopup>(base.transform).GetComponent<TooltipPopup>();
			MessagePopup = ElementFactory.CreateHeadedPanel<MessagePopup>(base.transform).GetComponent<MessagePopup>();
			ConfirmPopup = ElementFactory.CreateHeadedPanel<ConfirmPopup>(base.transform).GetComponent<ConfirmPopup>();
			_popups.Add(TooltipPopup);
			_popups.Add(MessagePopup);
			_popups.Add(ConfirmPopup);
		}

		protected virtual void HideAllPopups()
		{
			foreach (BasePopup popup in _popups)
			{
				popup.Hide();
			}
		}
	}
}
