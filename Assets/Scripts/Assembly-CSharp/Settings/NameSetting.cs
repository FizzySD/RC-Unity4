using SimpleJSONFixed;

namespace Settings
{
	internal class NameSetting : StringSetting
	{
		public int MaxStrippedLength = int.MaxValue;

		public NameSetting()
			: base(string.Empty)
		{
		}

		public NameSetting(string defaultValue, int maxLength = int.MaxValue, int maxStrippedLength = int.MaxValue)
			: base(defaultValue)
		{
			MaxLength = maxLength;
			MaxStrippedLength = maxStrippedLength;
		}

		public override void DeserializeFromJsonObject(JSONNode json)
		{
			base.Value = json.Value;
		}

		public override JSONNode SerializeToJsonObject()
		{
			return new JSONString(base.Value);
		}

		protected override string SanitizeValue(string value)
		{
			if (value.Length > MaxLength)
			{
				return value.Substring(0, MaxLength);
			}
			string text = value.StripHex();
			if (text.Length > MaxStrippedLength)
			{
				return text.Substring(0, MaxStrippedLength);
			}
			return value;
		}
	}
}
