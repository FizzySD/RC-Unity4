using System;
using System.Collections;
using System.Collections.Generic;
using ApplicationManagers;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class BasePanel : MonoBehaviour
	{
		protected Transform SinglePanel;

		protected Transform DoublePanelLeft;

		protected Transform DoublePanelRight;

		protected List<BasePopup> _popups = new List<BasePopup>();

		protected GameObject _currentCategoryPanel;

		protected StringSetting _currentCategoryPanelName = new StringSetting(string.Empty);

		protected Dictionary<string, Type> _categoryPanelTypes = new Dictionary<string, Type>();

		public BasePanel Parent;

		protected virtual string ThemePanel
		{
			get
			{
				return "DefaultPanel";
			}
		}

		protected virtual float Width
		{
			get
			{
				return 800f;
			}
		}

		protected virtual float Height
		{
			get
			{
				return 600f;
			}
		}

		protected virtual float BorderVerticalPadding
		{
			get
			{
				return 0f;
			}
		}

		protected virtual float BorderHorizontalPadding
		{
			get
			{
				return 0f;
			}
		}

		protected virtual int VerticalPadding
		{
			get
			{
				return 30;
			}
		}

		protected virtual int HorizontalPadding
		{
			get
			{
				return 40;
			}
		}

		protected virtual float VerticalSpacing
		{
			get
			{
				return 30f;
			}
		}

		protected virtual TextAnchor PanelAlignment
		{
			get
			{
				return TextAnchor.UpperLeft;
			}
		}

		protected virtual bool DoublePanel
		{
			get
			{
				return false;
			}
		}

		protected virtual bool DoublePanelDivider
		{
			get
			{
				return true;
			}
		}

		protected virtual bool ScrollBar
		{
			get
			{
				return false;
			}
		}

		protected virtual bool CategoryPanel
		{
			get
			{
				return false;
			}
		}

		protected virtual bool UseLastCategory
		{
			get
			{
				return true;
			}
		}

		protected virtual bool HasPremadeContent
		{
			get
			{
				return false;
			}
		}

		protected virtual string DefaultCategoryPanel
		{
			get
			{
				return string.Empty;
			}
		}

		protected void OnEnable()
		{
			if (base.transform.Find("Border") != null)
			{
				base.transform.Find("Border").GetComponent<CanvasGroup>().blocksRaycasts = false;
			}
			if (_currentCategoryPanel != null)
			{
				_currentCategoryPanel.SetActive(true);
			}
		}

		public virtual void Setup(BasePanel parent = null)
		{
			Parent = parent;
			base.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, Height);
			if (!CategoryPanel && !HasPremadeContent)
			{
				if (DoublePanel)
				{
					GameObject doublePanel = CreateDoublePanel(ScrollBar, DoublePanelDivider);
					DoublePanelLeft = GetDoublePanelLeftTransform(doublePanel);
					DoublePanelRight = GetDoublePanelRightTransform(doublePanel);
				}
				else
				{
					SinglePanel = GetSinglePanelTransform(CreateSinglePanel(ScrollBar));
				}
			}
			else if (HasPremadeContent)
			{
				SetupPremadePanel();
			}
			SetupPopups();
			if (CategoryPanel)
			{
				RegisterCategoryPanels();
				string lastcategory = UIManager.GetLastcategory(GetType());
				if (UseLastCategory && lastcategory != string.Empty)
				{
					SetCategoryPanel(lastcategory);
				}
				else
				{
					SetCategoryPanel(DefaultCategoryPanel);
				}
			}
		}

		public virtual void Show()
		{
			base.gameObject.SetActive(true);
		}

		public virtual void Hide()
		{
			HideAllPopups();
			base.gameObject.SetActive(false);
		}

		public virtual void SyncSettingElements()
		{
			BaseSettingElement[] componentsInChildren = GetComponentsInChildren<BaseSettingElement>();
			foreach (BaseSettingElement baseSettingElement in componentsInChildren)
			{
				baseSettingElement.SyncElement();
			}
		}

		protected virtual void SetupPremadePanel()
		{
			if (DoublePanel)
			{
				GameObject gameObject = base.transform.Find("DoublePanelContent").gameObject;
				DoublePanelLeft = GetDoublePanelLeftTransform(gameObject);
				DoublePanelRight = GetDoublePanelRightTransform(gameObject);
				BindPanel(gameObject, ScrollBar);
				SetPanelPadding(GetDoublePanelLeftTransform(gameObject).gameObject);
				SetPanelPadding(GetDoublePanelRightTransform(gameObject).gameObject);
			}
			else
			{
				GameObject gameObject2 = base.transform.Find("SinglePanelContent").gameObject;
				SinglePanel = GetSinglePanelTransform(gameObject2);
				BindPanel(gameObject2, ScrollBar);
				SetPanelPadding(GetSinglePanelTransform(gameObject2).gameObject);
			}
		}

		protected virtual void SetupPopups()
		{
		}

		protected virtual void HideAllPopups()
		{
			foreach (BasePopup popup in _popups)
			{
				popup.Hide();
			}
		}

		protected virtual void RegisterCategoryPanels()
		{
		}

		public virtual void SetCategoryPanel(string name)
		{
			HideAllPopups();
			if (_currentCategoryPanel != null)
			{
				UnityEngine.Object.Destroy(_currentCategoryPanel);
			}
			Type t = _categoryPanelTypes[name];
			_currentCategoryPanelName.Value = name;
			_currentCategoryPanel = ElementFactory.CreateDefaultPanel(base.transform, t, true);
			_currentCategoryPanel.SetActive(false);
			StartCoroutine(WaitAndEnableCategoryPanel());
			UIManager.SetLastCategory(GetType(), name);
		}

		private IEnumerator WaitAndEnableCategoryPanel()
		{
			yield return new WaitForEndOfFrame();
			_currentCategoryPanel.SetActive(true);
		}

		public string GetCurrentCategoryName()
		{
			return _currentCategoryPanelName.Value;
		}

		public void RebuildCategoryPanel()
		{
			SetCategoryPanel(_currentCategoryPanelName);
		}

		public void SetCategoryPanel(StringSetting setting)
		{
			SetCategoryPanel(setting.Value);
		}

		protected GameObject CreateHorizontalDivider(Transform parent, float height = 1f)
		{
			return ElementFactory.CreateHorizontalLine(width: (!DoublePanel) ? (GetPanelWidth() - (float)HorizontalPadding * 2f) : (GetPanelWidth() * 0.5f - (float)HorizontalPadding * 2f), parent: parent, style: new ElementStyle(24, 120f, ThemePanel), height: height);
		}

		protected Transform GetSinglePanelTransform(GameObject singlePanel)
		{
			return singlePanel.transform.Find("ScrollView/Panel");
		}

		protected Transform GetDoublePanelLeftTransform(GameObject doublePanel)
		{
			return doublePanel.transform.Find("ScrollView/LeftPanel");
		}

		protected Transform GetDoublePanelRightTransform(GameObject doublePanel)
		{
			return doublePanel.transform.Find("ScrollView/RightPanel");
		}

		protected GameObject CreateSinglePanel(bool scrollBar)
		{
			GameObject gameObject = AssetBundleManager.InstantiateAsset<GameObject>("SinglePanelContent");
			GetSinglePanelTransform(gameObject).GetComponent<LayoutElement>().preferredWidth = GetPanelWidth();
			BindPanel(gameObject, scrollBar);
			SetPanelPadding(GetSinglePanelTransform(gameObject).gameObject);
			return gameObject;
		}

		protected GameObject CreateDoublePanel(bool scrollBar, bool divider)
		{
			GameObject gameObject = AssetBundleManager.InstantiateAsset<GameObject>("DoublePanelContent");
			GetDoublePanelLeftTransform(gameObject).GetComponent<LayoutElement>().preferredWidth = GetPanelWidth() * 0.5f;
			GetDoublePanelRightTransform(gameObject).GetComponent<LayoutElement>().preferredWidth = GetPanelWidth() * 0.5f;
			if (divider)
			{
				Transform transform = gameObject.transform.Find("ScrollView/VerticalLine");
				transform.gameObject.AddComponent<VerticalLineScaler>();
			}
			else
			{
				gameObject.transform.Find("ScrollView/VerticalLine").gameObject.SetActive(false);
			}
			BindPanel(gameObject, scrollBar);
			SetPanelPadding(GetDoublePanelLeftTransform(gameObject).gameObject);
			SetPanelPadding(GetDoublePanelRightTransform(gameObject).gameObject);
			return gameObject;
		}

		protected virtual void BindPanel(GameObject panel, bool scrollBar)
		{
			panel.transform.SetParent(base.gameObject.transform, false);
			panel.transform.localPosition = Vector3.zero;
			float panelHeight = GetPanelHeight();
			panel.GetComponent<RectTransform>().sizeDelta = new Vector2(GetPanelWidth(), panelHeight);
			panel.transform.Find("ScrollView").GetComponent<LayoutElement>().minHeight = panelHeight;
			Scrollbar component = panel.transform.Find("Scrollbar").GetComponent<Scrollbar>();
			component.value = 1f;
			if (!scrollBar)
			{
				component.gameObject.SetActive(false);
			}
			panel.GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "BackgroundColor");
			component.colors = UIManager.GetThemeColorBlock(ThemePanel, "MainBody", "Scrollbar");
			component.GetComponent<Image>().color = UIManager.GetThemeColor(ThemePanel, "MainBody", "ScrollbarBackgroundColor");
		}

		protected void SetPanelPadding(GameObject panel)
		{
			panel.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(HorizontalPadding, HorizontalPadding, VerticalPadding, VerticalPadding);
			panel.GetComponent<VerticalLayoutGroup>().spacing = VerticalSpacing;
			panel.GetComponent<VerticalLayoutGroup>().childAlignment = PanelAlignment;
		}

		protected virtual float GetPanelWidth()
		{
			return Width - BorderHorizontalPadding * 2f;
		}

		protected virtual float GetPanelHeight()
		{
			return Height - BorderVerticalPadding * 2f;
		}
	}
}
