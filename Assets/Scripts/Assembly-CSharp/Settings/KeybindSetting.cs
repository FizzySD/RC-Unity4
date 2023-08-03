using System.Collections.Generic;
using SimpleJSONFixed;
using UnityEngine;

namespace Settings
{
	internal class KeybindSetting : BaseSetting
	{
		public List<InputKey> InputKeys = new List<InputKey>();

		protected string[] _defaultKeyStrings;

		public KeybindSetting(string[] defaultKeyStrings)
		{
			_defaultKeyStrings = defaultKeyStrings;
			SetDefault();
		}

		public override void SetDefault()
		{
			LoadFromStringArray(_defaultKeyStrings);
		}

		protected void LoadFromStringArray(string[] keyStrings)
		{
			InputKeys.Clear();
			foreach (string keyStr in keyStrings)
			{
				InputKey item = new InputKey(keyStr);
				InputKeys.Add(item);
			}
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (InputKey inputKey in InputKeys)
			{
				if (!inputKey.IsNone())
				{
					list.Add(inputKey.ToString());
				}
			}
			if (list.Count == 0)
			{
				return "None";
			}
			return string.Join(" / ", list.ToArray());
		}

		public bool Contains(InputKey key)
		{
			foreach (InputKey inputKey in InputKeys)
			{
				if (inputKey.Equals(key))
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(KeyCode key)
		{
			foreach (InputKey inputKey in InputKeys)
			{
				if (inputKey.MatchesKeyCode(key))
				{
					return true;
				}
			}
			return false;
		}

		public bool GetKeyDown()
		{
			foreach (InputKey inputKey in InputKeys)
			{
				if (inputKey.GetKeyDown())
				{
					return true;
				}
			}
			return false;
		}

		public bool GetKey()
		{
			foreach (InputKey inputKey in InputKeys)
			{
				if (inputKey.GetKey())
				{
					return true;
				}
			}
			return false;
		}

		public bool GetKeyUp()
		{
			foreach (InputKey inputKey in InputKeys)
			{
				if (inputKey.GetKeyUp())
				{
					return true;
				}
			}
			return false;
		}

		public override JSONNode SerializeToJsonObject()
		{
			JSONArray jSONArray = new JSONArray();
			foreach (InputKey inputKey in InputKeys)
			{
				jSONArray.Add(new JSONString(inputKey.ToString()));
			}
			return jSONArray;
		}

		public override void DeserializeFromJsonObject(JSONNode json)
		{
			List<string> list = new List<string>();
			JSONArray asArray = json.AsArray;
			JSONNode.Enumerator enumerator = asArray.GetEnumerator();
			while (enumerator.MoveNext())
			{
				JSONString jSONString = (JSONString)(JSONNode)enumerator.Current;
				list.Add(jSONString.Value);
			}
			LoadFromStringArray(list.ToArray());
		}
	}
}
