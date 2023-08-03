using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class EditProfilePopup : BasePopup
	{
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
				return 730f;
			}
		}

		protected override float Height
		{
			get
			{
				return 660f;
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
				return "Profile";
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			SetupBottomButtons();
		}

		protected override void SetupTopButtons()
		{
			ElementStyle style = new ElementStyle(28, 120f, ThemePanel);
			string[] array = new string[2] { "Profile", "Stats" };
			foreach (string buttonName in array)
			{
				GameObject gameObject = ElementFactory.CreateCategoryButton(TopBar, style, UIManager.GetLocaleCommon(buttonName), delegate
				{
					SetCategoryPanel(buttonName);
				});
				_topButtons.Add(buttonName, gameObject.GetComponent<Button>());
			}
			base.SetupTopButtons();
		}

		protected override void RegisterCategoryPanels()
		{
			_categoryPanelTypes.Add("Profile", typeof(EditProfileProfilePanel));
			_categoryPanelTypes.Add("Stats", typeof(EditProfileStatsPanel));
		}

		protected override void SetupPopups()
		{
			base.SetupPopups();
		}

		private void SetupBottomButtons()
		{
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			string[] array = new string[1] { "Save" };
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
			if (name == "Save")
			{
				SettingsManager.ProfileSettings.Save();
				Hide();
			}
		}
	}
}
