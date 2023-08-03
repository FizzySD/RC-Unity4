namespace Settings
{
	internal class InputSettings : SaveableSettingsContainer
	{
		public GeneralInputSettings General = new GeneralInputSettings();

		public HumanInputSettings Human = new HumanInputSettings();

		public TitanInputSettings Titan = new TitanInputSettings();

		public ShifterInputSettings Shifter = new ShifterInputSettings();

		public InteractionInputSettings Interaction = new InteractionInputSettings();

		public RCEditorInputSettings RCEditor = new RCEditorInputSettings();

		protected override string FileName
		{
			get
			{
				return "Input.json";
			}
		}
	}
}
