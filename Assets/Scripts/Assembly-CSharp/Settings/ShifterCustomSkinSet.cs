namespace Settings
{
	internal class ShifterCustomSkinSet : BaseSetSetting
	{
        public ShifterCustomSkinSet()
        {
            // Inizializzazione o operazioni nel costruttore se necessario
        }
        public StringSetting Eren = new StringSetting(string.Empty, 200);

		public StringSetting Annie = new StringSetting(string.Empty, 200);

		public StringSetting Colossal = new StringSetting(string.Empty, 200);
	}
}
