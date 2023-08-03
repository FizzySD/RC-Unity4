using Utility;

namespace Map
{
	internal class MapScriptOptions : BaseCSVRow
	{
		public string Version;

		protected override bool NamedParams
		{
			get
			{
				return true;
			}
		}
	}
}
