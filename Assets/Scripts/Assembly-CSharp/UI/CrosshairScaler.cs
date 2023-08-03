using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class CrosshairScaler : IgnoreScaler
	{
		public override void ApplyScale()
		{
			base.ApplyScale();
			float value = SettingsManager.UISettings.CrosshairScale.Value;
			RectTransform component = GetComponent<RectTransform>();
			Vector3 localScale = component.localScale;
			component.localScale = new Vector2(localScale.x * value, localScale.y * value);
			int num = 16;
			if (value > 1f)
			{
				num = (int)(16f * value);
			}
			value = 16f / (float)num;
			base.transform.Find("DefaultLabel").GetComponent<Text>().fontSize = num;
			base.transform.Find("DefaultLabel").GetComponent<RectTransform>().localScale = new Vector2(value, value);
		}
	}
}
