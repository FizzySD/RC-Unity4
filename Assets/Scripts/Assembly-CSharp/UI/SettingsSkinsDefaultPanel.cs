namespace UI
{
	internal class SettingsSkinsDefaultPanel : SettingsCategoryPanel
	{
		protected override bool ScrollBar
		{
			get
			{
				return true;
			}
		}

		protected override float VerticalSpacing
		{
			get
			{
				return 20f;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			base.Setup(parent);
			SettingsSkinsPanel settingsSkinsPanel = (SettingsSkinsPanel)parent;
			settingsSkinsPanel.CreateCommonSettings(DoublePanelLeft, DoublePanelRight);
			CreateHorizontalDivider(DoublePanelRight);
			settingsSkinsPanel.CreateSkinStringSettings(DoublePanelLeft, DoublePanelRight);
		}
	}
}
