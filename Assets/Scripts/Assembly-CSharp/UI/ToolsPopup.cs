using UnityEngine;

namespace UI
{
	internal class ToolsPopup : BasePopup
	{
		protected override string Title
		{
			get
			{
				return UIManager.GetLocale("MainMenu", "ToolsPopup", "Title");
			}
		}

		protected override float Width
		{
			get
			{
				return 280f;
			}
		}

		protected override float Height
		{
			get
			{
				return 355f;
			}
		}

		protected override float VerticalSpacing
		{
			get
			{
				return 20f;
			}
		}

		protected override int VerticalPadding
		{
			get
			{
				return 20;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			string category = "MainMenu";
			string subCategory = "ToolsPopup";
			float elementWidth = 210f;
			ElementStyle style = new ElementStyle(ButtonFontSize, 120f, ThemePanel);
			ElementFactory.CreateDefaultButton(BottomBar, style, UIManager.GetLocaleCommon("Back"), 0f, 0f, delegate
			{
				OnButtonClick("Back");
			});
			ElementFactory.CreateDefaultButton(SinglePanel, style, UIManager.GetLocale(category, subCategory, "ButtonMapEditor"), elementWidth, 0f, delegate
			{
				OnButtonClick("MapEditor");
			});
			ElementFactory.CreateDefaultButton(SinglePanel, style, UIManager.GetLocale(category, subCategory, "ButtonCharacterEditor"), elementWidth, 0f, delegate
			{
				OnButtonClick("CharacterEditor");
			});
			ElementFactory.CreateDefaultButton(SinglePanel, style, UIManager.GetLocale(category, subCategory, "ButtonSnapshotViewer"), elementWidth, 0f, delegate
			{
				OnButtonClick("SnapshotViewer");
			});
		}

		protected void OnButtonClick(string name)
		{
			switch (name)
			{
			case "MapEditor":
				FengGameManagerMKII.settingsOld[64] = 101;
				Application.LoadLevel(2);
				break;
			case "CharacterEditor":
				Application.LoadLevel("characterCreation");
				break;
			case "SnapshotViewer":
				Application.LoadLevel("SnapShot");
				break;
			case "Back":
				Hide();
				break;
			}
		}
	}
}
