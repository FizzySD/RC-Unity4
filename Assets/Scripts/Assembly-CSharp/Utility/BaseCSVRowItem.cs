namespace Utility
{
	internal class BaseCSVRowItem : BaseCSVObject
	{
		protected override char Delimiter
		{
			get
			{
				return '|';
			}
		}
	}
}
