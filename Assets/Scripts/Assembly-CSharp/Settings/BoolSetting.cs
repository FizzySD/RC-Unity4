using SimpleJSONFixed;

namespace Settings
{
	internal class BoolSetting : TypedSetting<bool>
	{
		public BoolSetting()
			: base(false)
		{
		}

		public BoolSetting(bool defaultValue)
			: base(defaultValue)
		{
		}

		public override void DeserializeFromJsonObject(JSONNode json)
		{
			base.Value = json.AsBool;
		}

		public override JSONNode SerializeToJsonObject()
		{
			return new JSONBool(base.Value);
		}
	}
}
