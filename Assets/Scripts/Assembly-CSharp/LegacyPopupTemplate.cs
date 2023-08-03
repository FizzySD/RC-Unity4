using UnityEngine;

internal class LegacyPopupTemplate
{
	private Color BorderColor;

	private Texture2D BackgroundTexture;

	private float PositionX;

	private float PositionY;

	private float Width;

	private float Height;

	private float BorderThickness;

	private float Padding;

	private Color ButtonColor;

	public LegacyPopupTemplate(Color borderColor, Texture2D bgTexture, Color buttonColor, float x, float y, float w, float h, float borderThickness)
	{
		BorderColor = borderColor;
		BackgroundTexture = bgTexture;
		ButtonColor = buttonColor;
		PositionX = x - w / 2f;
		PositionY = y - h / 2f;
		Width = w;
		Height = h;
		BorderThickness = borderThickness;
	}

	public void DrawPopup(string message, float messageWidth, float messageHeight)
	{
		DrawPopupBackground();
		float num = (Width - messageWidth) * 0.5f;
		float num2 = (Height - messageHeight) * 0.5f;
		GUI.Label(new Rect(PositionX + num, PositionY + num2, messageWidth, messageHeight), message);
	}

	public bool DrawPopupWithButton(string message, float messageWidth, float messageHeight, string buttonMessage, float buttonWidth, float buttonHeight)
	{
		DrawPopupBackground();
		float num = (Width - messageWidth) * 0.5f;
		float num2 = (Width - buttonWidth) * 0.5f;
		float num3 = (Height - messageHeight - buttonHeight) / 3f;
		GUI.Label(new Rect(PositionX + num, PositionY + num3, messageWidth, messageHeight), message);
		float left = PositionX + num2;
		float top = PositionY + Height - buttonHeight - num3;
		GUI.backgroundColor = ButtonColor;
		return GUI.Button(new Rect(left, top, buttonWidth, buttonHeight), buttonMessage);
	}

	public bool[] DrawPopupWithTwoButtons(string message, float messageWidth, float messageHeight, string button1Message, float button1Width, string button2Message, float button2Width, float buttonHeight)
	{
		DrawPopupBackground();
		float num = (Width - messageWidth) * 0.5f;
		float num2 = (Width - button1Width - button2Width) / 3f;
		float num3 = (Height - messageHeight - buttonHeight) / 3f;
		GUI.Label(new Rect(PositionX + num, PositionY + num3, messageWidth, messageHeight), message);
		float num4 = PositionX + num2;
		float left = num4 + button1Width + num2;
		float top = PositionY + Height - buttonHeight - num3;
		GUI.backgroundColor = ButtonColor;
		return new bool[2]
		{
			GUI.Button(new Rect(num4, top, button1Width, buttonHeight), button1Message),
			GUI.Button(new Rect(left, top, button2Width, buttonHeight), button2Message)
		};
	}

	private void DrawPopupBackground()
	{
		GUI.backgroundColor = BorderColor;
		GUI.Box(new Rect(PositionX, PositionY, Width, Height), string.Empty);
		GUI.DrawTexture(new Rect(PositionX + BorderThickness, PositionY + BorderThickness, Width - 2f * BorderThickness, Height - 2f * BorderThickness), BackgroundTexture);
	}
}
