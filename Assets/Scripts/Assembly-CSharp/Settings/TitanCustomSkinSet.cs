namespace Settings
{
	internal class TitanCustomSkinSet : BaseSetSetting
    {
		public BoolSetting RandomizedPairs = new BoolSetting(false);

		public ListSetting<StringSetting> Hairs = new ListSetting<StringSetting>(new StringSetting(string.Empty, 200), 5);

		public ListSetting<IntSetting> HairModels = new ListSetting<IntSetting>(new IntSetting(0, 0), 5);

		public ListSetting<StringSetting> Bodies = new ListSetting<StringSetting>(new StringSetting(string.Empty, 200), 5);

		public ListSetting<StringSetting> Eyes = new ListSetting<StringSetting>(new StringSetting(string.Empty, 200), 5);

        public TitanCustomSkinSet()
        {
            // Inizializzazione o operazioni nel costruttore se necessario
        }
        protected override bool Validate()
		{
			if (Hairs.Value.Count == 5 && HairModels.Value.Count == 5 && Bodies.Value.Count == 5)
			{
				return Eyes.Value.Count == 5;
			}
			return false;
		}
	}
}
