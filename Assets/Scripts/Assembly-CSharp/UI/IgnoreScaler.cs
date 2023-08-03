using Settings;
using UnityEngine;

namespace UI
{
	internal class IgnoreScaler : BaseScaler
	{
		public float Scale = 1f;

		public override void ApplyScale()
		{
			float value = SettingsManager.UISettings.UIMasterScale.Value;
			RectTransform component = GetComponent<RectTransform>();
			Scale = 1f / value;
			component.localScale = new Vector2(Scale, Scale);
		}
	}
}
