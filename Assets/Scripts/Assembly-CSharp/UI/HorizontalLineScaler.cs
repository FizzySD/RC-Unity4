using UnityEngine;

namespace UI
{
	internal class HorizontalLineScaler : BaseScaler
	{
		public override void ApplyScale()
		{
			float currentCanvasScale = UIManager.CurrentCanvasScale;
			RectTransform component = GetComponent<RectTransform>();
			float num = 1f;
			if (num * currentCanvasScale < 1f)
			{
				num = 1f / currentCanvasScale;
			}
			component.sizeDelta = new Vector2(component.sizeDelta.x, num);
		}
	}
}
