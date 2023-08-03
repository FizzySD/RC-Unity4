using UnityEngine;

namespace UI
{
	internal class SettingsKeybindsPanel : SettingsCategoryPanel
	{
		protected string[] _categories = new string[6] { "General", "Human", "Titan", "Shifter", "Interaction", "RC Editor" };

		protected override bool CategoryPanel
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
				return "General";
			}
		}

		public void CreateGategoryDropdown(Transform panel)
		{
			ElementStyle style = new ElementStyle(24, 140f, ThemePanel);
			ElementFactory.CreateDropdownSetting(panel, style, _currentCategoryPanelName, "Category", _categories, "", 260f, 40f, 300f, null, delegate
			{
				RebuildCategoryPanel();
			});
		}

		protected override void RegisterCategoryPanels()
		{
			string[] categories = _categories;
			foreach (string key in categories)
			{
				_categoryPanelTypes.Add(key, typeof(SettingsKeybindsDefaultPanel));
			}
		}
	}
}
