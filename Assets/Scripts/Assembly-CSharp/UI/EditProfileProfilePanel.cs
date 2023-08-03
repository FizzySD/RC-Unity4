using Settings;

namespace UI
{
	internal class EditProfileProfilePanel : BasePanel
	{
		protected override float Width
		{
			get
			{
				return 720f;
			}
		}

		protected override float Height
		{
			get
			{
				return 520f;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			ProfileSettings profileSettings = SettingsManager.ProfileSettings;
			ElementStyle style = new ElementStyle(24, 120f, ThemePanel);
			ElementFactory.CreateInputSetting(SinglePanel, style, profileSettings.Name, UIManager.GetLocaleCommon("Name"), "", 200f);
			ElementFactory.CreateInputSetting(SinglePanel, style, profileSettings.Guild, UIManager.GetLocaleCommon("Guild"), "", 200f);
		}
	}
}
