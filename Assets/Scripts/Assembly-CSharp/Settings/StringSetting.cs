using SimpleJSONFixed;

namespace Settings
{
	internal class StringSetting : TypedSetting<string>
	{
		public int MaxLength = int.MaxValue;

		public StringSetting()
			: base(string.Empty)
		{
		}

		public StringSetting(string defaultValue, int maxLength = int.MaxValue)
			: base(defaultValue)
		{
			MaxLength = maxLength;
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
			return value;
		}
	}
}
