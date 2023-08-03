using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class QuestPopup : BasePopup
	{
		public StringSetting TierSelection = new StringSetting("Bronze");

		public StringSetting CompletedSelection = new StringSetting("In Progress");

		protected override string Title
		{
			get
			{
				return string.Empty;
			}
		}

		protected override float Width
		{
			get
			{
				return 990f;
			}
		}

		protected override float Height
		{
			get
			{
				return 740f;
			}
		}

		protected override bool CategoryPanel
		{
			get
			{
				return true;
			}
		}

		protected override bool CategoryButtons
		{
			get
			{
				return true;
			}
		}

		protected override string DefaultCategoryPanel
		{
			get
			{
				return "Daily";
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			SetupBottomButtons();
		}

		public void CreateAchievmentDropdowns(Transform panel)
		{
			ElementStyle style = new ElementStyle(24, 0f, ThemePanel);
			ElementFactory.CreateDropdownSetting(panel, style, TierSelection, "", new string[3] { "Bronze", "Silver", "Gold" }, "", 180f, 40f, 300f, null, delegate
			{
				RebuildCategoryPanel();
			});
			ElementFactory.CreateDropdownSetting(panel, style, CompletedSelection, "", new string[2] { "In Progress", "Completed" }, "", 180f, 40f, 300f, null, delegate
			{
				RebuildCategoryPanel();
			});
		}

		protected override void SetupTopButtons()
		{
			ElementStyle style = new ElementStyle(28, 120f, ThemePanel);
			string[] array = new string[3] { "Daily", "Weekly", "Achievments" };
			foreach (string buttonName in array)
			{
				GameObject gameObject = ElementFactory.CreateCategoryButton(title: (!(buttonName == "Daily") && !(buttonName == "Weekly")) ? UIManager.GetLocaleCommon(buttonName) : UIManager.GetLocale("MainMenu", "QuestsPopup", buttonName), parent: TopBar, style: style, onClick: delegate
				{
					SetCategoryPanel(buttonName);
				});
				_topButtons.Add(buttonName, gameObject.GetComponent<Button>());
			}
			base.SetupTopButtons();
		}

		protected override void RegisterCategoryPanels()
		{
			_categoryPanelTypes.Add("Daily", typeof(QuestDailyPanel));
			_categoryPanelTypes.Add("Weekly", typeof(QuestWeeklyPanel));
			_categoryPanelTypes.Add("Achievments", typeof(QuestAchievmentsPanel));
		}

		protected override void SetupPopups()
		{
			base.SetupPopups();
		}

		private void SetupBottomButtons()
		{
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			string[] array = new string[1] { "Back" };
			foreach (string buttonName in array)
			{
				GameObject gameObject = ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon(buttonName), 0f, 0f, delegate
				{
					OnBottomBarButtonClick(buttonName);
				});
			}
		}

		private void OnBottomBarButtonClick(string name)
		{
			if (name == "Back")
			{
				Hide();
			}
		}
	}
}
