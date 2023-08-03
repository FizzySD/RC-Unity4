using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace ApplicationManagers
{
	internal class DebugConsole : MonoBehaviour
	{
		private static DebugConsole _instance;

		private static bool _enabled;

		private static LinkedList<string> _messages = new LinkedList<string>();

		private static int _currentCharCount = 0;

		private static Vector2 _scrollPosition = Vector2.zero;

		private static string _inputLine = string.Empty;

		private static bool _needResetScroll;

		private const int MaxMessages = 100;

		private const int MaxChars = 5000;

		private const int PositionX = 20;

		private const int PositionY = 20;

		private const int Width = 400;

		private const int Height = 300;

		private const int InputHeight = 25;

		private const int Padding = 10;

		private const string InputControlName = "DebugInput";

		public static void Init()
		{
			_instance = SingletonFactory.CreateSingleton(_instance);
			Application.RegisterLogCallback(OnUnityDebugLog);
		}

		private static void OnUnityDebugLog(string log, string stackTrace, LogType type)
		{
			AddMessage(stackTrace);
			AddMessage(log);
		}

		private static void AddMessage(string message)
		{
			_messages.AddLast(message);
			_currentCharCount += message.Length;
			while (_messages.Count > 100 || _currentCharCount > 5000)
			{
				_currentCharCount -= _messages.First.Value.Length;
				_messages.RemoveFirst();
			}
			_needResetScroll = true;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F11))
			{
				_enabled = !_enabled;
			}
		}

		private void OnGUI()
		{
			if (_enabled)
			{
				GUI.depth = 1;
				GUI.Box(new Rect(20f, 20f, 400f, 300f), "");
				DrawMessageWindow();
				DrawInputWindow();
				HandleInput();
				GUI.depth = 0;
			}
		}

		private static void DrawMessageWindow()
		{
			int num = 30;
			int num2 = 30;
			int num3 = 380;
			GUI.Label(new Rect(num, num2, num3, 25f), "Debug Console (Press F11 to hide)");
			num2 += 35;
			int num4 = 210;
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.box);
			string text = "";
			foreach (string message in _messages)
			{
				text = text + message + "\n";
			}
			int num5 = num3 - 20;
			int num6 = (int)gUIStyle.CalcHeight(new GUIContent(text), num5) + 10;
			_scrollPosition = GUI.BeginScrollView(new Rect(num, num2, num3, num4), _scrollPosition, new Rect(num, num2, num5, num6));
			GUI.Label(new Rect(num, num2, num5, num6), text);
			if (_needResetScroll)
			{
				_needResetScroll = false;
				_scrollPosition = new Vector2(0f, num6);
			}
			GUI.EndScrollView();
		}

		private static void DrawInputWindow()
		{
			int num = 285;
			GUI.SetNextControlName("DebugInput");
			_inputLine = GUI.TextField(new Rect(30f, num, 380f, 25f), _inputLine);
		}

		private static void HandleInput()
		{
			if (GUI.GetNameOfFocusedControl() == "DebugInput")
			{
				if (!IsEnterUp())
				{
					return;
				}
				if (_inputLine != string.Empty)
				{
					Debug.Log(_inputLine);
					if (_inputLine.StartsWith("/"))
					{
						DebugTesting.RunDebugCommand(_inputLine.Substring(1));
					}
					else
					{
						Debug.Log("Invalid debug command.");
					}
					_inputLine = string.Empty;
				}
				GUI.FocusControl(string.Empty);
			}
			else if (IsEnterUp())
			{
				GUI.FocusControl("DebugInput");
			}
		}

		private static bool IsEnterUp()
		{
			if (Event.current.type == EventType.KeyUp)
			{
				if (Event.current.keyCode != KeyCode.Return)
				{
					return Event.current.keyCode == KeyCode.KeypadEnter;
				}
				return true;
			}
			return false;
		}
	}
}
