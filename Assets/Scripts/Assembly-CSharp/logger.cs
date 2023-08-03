using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class logger : Photon.MonoBehaviour
{
	public int ScrollRPCConsole;

	private bool AlignBottom = true;

	public static int guiLines;

	public static Rect GuiRect = new Rect(650f, 100f, 300f, 470f);

	public static bool Joined = true;

	public static bool RPCConsole = true;

	public static List<string> messages = new List<string>();

	private static GUIStyle currentStyle;

	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = col;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	public static void addLINE(string newLine)
	{
		messages.Add(newLine);
	}

	public void OnGUI()
	{
		if (!RPCConsole)
		{
			return;
		}
		currentStyle = new GUIStyle(GUI.skin.box);
		currentStyle.alignment = TextAnchor.MiddleLeft;
		GuiRect = new Rect((float)Screen.width - 300f, Screen.height - 500, 300f, 470f);
		string text = string.Empty;
		GUI.SetNextControlName(string.Empty);
		GUILayout.BeginArea(GuiRect);
		GUILayout.FlexibleSpace();
		int num = Convert.ToInt32(guiLines);
		if (messages.Count < 15)
		{
			for (num = 0; num < messages.Count; num++)
			{
				text = text + messages[num] + "\n";
			}
		}
		else
		{
			for (int i = messages.Count - 15 - 10 * ScrollRPCConsole; i < messages.Count - 10 * ScrollRPCConsole; i++)
			{
				if (i >= 0)
				{
					text = text + messages[i] + "\n";
				}
			}
		}
		GUILayout.Label("");
		GUILayout.Label(text, currentStyle);
		GUILayout.EndArea();
		GUILayout.BeginArea(GuiRect);
		GUILayout.BeginHorizontal();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	public void setPosition()
	{
		bool alignBottom = AlignBottom;
	}

	public void Start()
	{
		setPosition();
		ScrollRPCConsole = 0;
		guiLines = 15;
		if (Joined)
		{
			Joined = true;
		}
		addLINE("<color=#ff00b4>W</color><color=#ec00c1>e</color><color=#d800cd>l</color><color=#c500da>c</color><color=#b200e6>o</color><color=#9e00f3>m</color><color=#8b00ff>e</color>, " + FengGameManagerMKII.nameField.hexColor());
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.PageUp) && messages.Count > 15 && ScrollRPCConsole <= messages.Count / 15)
		{
			ScrollRPCConsole++;
		}
		if (Input.GetKeyDown(KeyCode.PageDown) && messages.Count > 15)
		{
			ScrollRPCConsole--;
			if (ScrollRPCConsole < 0)
			{
				ScrollRPCConsole = 0;
			}
		}
	}

	internal static void addLINE(string v1, string v2)
	{
		throw new NotImplementedException();
	}
}
