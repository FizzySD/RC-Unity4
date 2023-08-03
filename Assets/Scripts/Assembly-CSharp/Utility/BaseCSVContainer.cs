namespace Utility
{
	internal class BaseCSVContainer : BaseCSVObject
	{
		protected override char Delimiter
		{
			get
			{
				return ';';
			}
		}

		protected virtual bool UseNewlines
		{
			get
			{
				return true;
			}
		}

		public override string Serialize()
		{
			string text = base.Serialize();
			if (UseNewlines)
			{
				text = InsertNewlines(text);
			}
			return text;
		}

		public virtual string InsertNewlines(string str)
		{
			string[] value = str.Split(Delimiter);
			return string.Join(Delimiter + "\n", value);
		}
	}
}
