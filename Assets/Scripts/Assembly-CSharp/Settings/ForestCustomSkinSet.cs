namespace Settings
{
	internal class ForestCustomSkinSet : BaseSetSetting
	{
		public ForestCustomSkinSet() 
		{
		
		}
		public BoolSetting RandomizedPairs = new BoolSetting(false);

		public ListSetting<StringSetting> TreeTrunks = new ListSetting<StringSetting>(new StringSetting(string.Empty, 200), 8);

		public ListSetting<StringSetting> TreeLeafs = new ListSetting<StringSetting>(new StringSetting(string.Empty, 200), 8);

		public StringSetting Ground = new StringSetting(string.Empty, 200);

		protected override bool Validate()
		{
			if (TreeTrunks.Value.Count == 8)
			{
				return TreeLeafs.Value.Count == 8;
			}
			return false;
		}
	}
}
