using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Anticheat
{
	internal static class ChatFilter
	{
		public static string FilterSizeTag(this string text)
		{
			MatchCollection matchCollection = Regex.Matches(text.ToLower(), "(<size=(.*?>))");
			List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
			foreach (Match match in matchCollection)
			{
				if (!list.Any((KeyValuePair<int, string> p) => p.Key == match.Index))
				{
					list.Add(new KeyValuePair<int, string>(match.Index, match.Value));
				}
			}
			foreach (KeyValuePair<int, string> item in list)
			{
				if (item.Value.StartsWith("<size=") && item.Value.Length > 9)
				{
					text = text.Remove(item.Key, item.Value.Length);
					text = text.Substring(0, item.Key) + "<size=20>" + text.Substring(item.Key, text.Length - item.Key);
				}
			}
			return text;
		}
	}
}
