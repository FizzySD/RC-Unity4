using System.Collections.Generic;
using Utility;

namespace Replay
{
	internal class ReplayScriptEvent : BaseCSVRow
	{
		public float Time;

		public string Category;

		public string Action;

		public List<string> Parameters = new List<string>();
	}
}
