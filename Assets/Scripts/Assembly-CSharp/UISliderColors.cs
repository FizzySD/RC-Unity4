using UnityEngine;

[AddComponentMenu("NGUI/Examples/Slider Colors")]
[RequireComponent(typeof(UISlider))]
[ExecuteInEditMode]
public class UISliderColors : MonoBehaviour
{
	public Color[] colors = new Color[3]
	{
		Color.red,
		Color.yellow,
		Color.green
	};

	private UISlider mSlider;

	public UISprite sprite;

	private void Start()
	{
		mSlider = GetComponent<UISlider>();
		Update();
	}

	private void Update()
	{
		if (!(sprite != null) || colors.Length == 0)
		{
			return;
		}
		float num = mSlider.sliderValue * (float)(colors.Length - 1);
		int num2 = Mathf.FloorToInt(num);
		Color color = colors[0];
		if (num2 >= 0)
		{
			if (num2 + 1 >= colors.Length)
			{
				color = ((num2 >= colors.Length) ? colors[colors.Length - 1] : colors[num2]);
			}
			else
			{
				float t = num - (float)num2;
				color = Color.Lerp(colors[num2], colors[num2 + 1], t);
			}
		}
		color.a = sprite.color.a;
		sprite.color = color;
	}
}
