using System.Collections;
using System.Threading;
using UnityEngine;

public class TextureScaler
{
	public class ThreadData
	{
		public Color[] TexColors;

		public Color[] NewColors;

		public int TexWidth;

		public int TexHeight;

		public int NewWidth;

		public int NewHeight;

		public ThreadData(Color[] texColors, Color[] newColors, int texWidth, int texHeight, int newWidth, int newHeight)
		{
			TexColors = texColors;
			NewColors = newColors;
			TexWidth = texWidth;
			TexHeight = texHeight;
			NewWidth = newWidth;
			NewHeight = newHeight;
		}
	}

	public static IEnumerator Scale(Texture2D tex, int newWidth, int newHeight)
	{
		Color[] pixels = tex.GetPixels();
		Color[] newColors = new Color[newWidth * newHeight];
		ThreadData parameter = new ThreadData(pixels, newColors, tex.width, tex.height, newWidth, newHeight);
		ParameterizedThreadStart start = BilinearScale;
		Thread thread = new Thread(start);
		thread.Start(parameter);
		while (thread.IsAlive)
		{
			yield return new WaitForEndOfFrame();
		}
		tex.Resize(newWidth, newHeight);
		tex.SetPixels(newColors);
		yield return new WaitForEndOfFrame();
		tex.Apply();
	}

	public static void BilinearScale(object obj)
	{
		ThreadData threadData = (ThreadData)obj;
		float num = 1f / ((float)threadData.NewWidth / (float)(threadData.TexWidth - 1));
		float num2 = 1f / ((float)threadData.NewHeight / (float)(threadData.TexHeight - 1));
		int texWidth = threadData.TexWidth;
		int newWidth = threadData.NewWidth;
		for (int i = 0; i < threadData.NewHeight; i++)
		{
			int num3 = (int)Mathf.Floor((float)i * num2);
			int num4 = num3 * texWidth;
			int num5 = (num3 + 1) * texWidth;
			int num6 = i * newWidth;
			for (int j = 0; j < newWidth; j++)
			{
				int num7 = (int)Mathf.Floor((float)j * num);
				float value = (float)j * num - (float)num7;
				Color[] texColors = threadData.TexColors;
				threadData.NewColors[num6 + j] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[num4 + num7], texColors[num4 + num7 + 1], value), ColorLerpUnclamped(texColors[num5 + num7], texColors[num5 + num7 + 1], value), (float)i * num2 - (float)num3);
			}
		}
	}

	private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
	{
		return new Color(c1.r + (c2.r - c1.r) * value, c1.g + (c2.g - c1.g) * value, c1.b + (c2.b - c1.b) * value, c1.a + (c2.a - c1.a) * value);
	}
}
