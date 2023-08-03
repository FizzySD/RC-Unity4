using SimpleJSONFixed;
using UnityEngine;

namespace Settings
{
	internal class ColorSetting : TypedSetting<Color>
	{
		public float MinAlpha;

		public ColorSetting()
			: base(Color.white)
		{
		}

		public ColorSetting(Color defaultValue, float minAlpha = 0f)
		{
			MinAlpha = minAlpha;
			DefaultValue = SanitizeValue(defaultValue);
			base.Value = DefaultValue;
		}

		protected override Color SanitizeValue(Color value)
		{
			value.r = Mathf.Clamp(value.r, 0f, 1f);
			value.g = Mathf.Clamp(value.g, 0f, 1f);
			value.b = Mathf.Clamp(value.b, 0f, 1f);
			value.a = Mathf.Clamp(value.a, MinAlpha, 1f);
			return value;
		}

		public override JSONNode SerializeToJsonObject()
		{
			JSONArray jSONArray = new JSONArray();
			jSONArray.Add(new JSONNumber(base.Value.r));
			jSONArray.Add(new JSONNumber(base.Value.g));
			jSONArray.Add(new JSONNumber(base.Value.b));
			jSONArray.Add(new JSONNumber(base.Value.a));
			return jSONArray;
		}

		public override void DeserializeFromJsonObject(JSONNode json)
		{
			JSONArray asArray = json.AsArray;
			base.Value = new Color(asArray[0].AsFloat, asArray[1].AsFloat, asArray[2].AsFloat, asArray[3].AsFloat);
		}
	}
}
