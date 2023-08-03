using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class InputFieldPasteable : InputField
	{
		protected bool IsModifier()
		{
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				if (!Input.GetKey(KeyCode.LeftCommand))
				{
					return Input.GetKey(KeyCode.RightCommand);
				}
				return true;
			}
			if (!Input.GetKey(KeyCode.LeftControl))
			{
				return Input.GetKey(KeyCode.RightControl);
			}
			return true;
		}

		protected bool IsCopy()
		{
			return Input.GetKeyDown(KeyCode.C);
		}

		protected bool IsPaste()
		{
			return Input.GetKeyDown(KeyCode.V);
		}

		protected bool IsCut()
		{
			return Input.GetKeyDown(KeyCode.X);
		}

		protected override void Append(char input)
		{
			if (Application.platform != RuntimePlatform.OSXPlayer || !IsModifier() || (!IsCopy() && !IsCut() && !IsPaste()))
			{
				base.Append(input);
			}
		}

		protected override void Append(string input)
		{
			if (Application.platform == RuntimePlatform.OSXPlayer && IsModifier() && (IsCopy() || IsCut()))
			{
				return;
			}
			if (base.multiLine && IsModifier() && IsPaste())
			{
				input = GetClipboard();
				int num = base.caretPosition;
				if (base.caretPosition != m_CaretSelectPosition && base.text.Length > 0)
				{
					int num2 = Math.Min(base.caretPosition, m_CaretSelectPosition);
					int num3 = Math.Max(base.caretPosition, m_CaretSelectPosition);
					if (num3 >= base.text.Length)
					{
						base.text = base.text.Substring(0, num2);
					}
					else
					{
						base.text = base.text.Substring(0, num2) + base.text.Substring(num3, base.text.Length - num3);
					}
					num = num2;
				}
				if (num >= base.text.Length || base.text.Length == 0)
				{
					base.text += input;
				}
				else
				{
					base.text = base.text.Substring(0, num) + input + base.text.Substring(num, base.text.Length - num);
				}
				base.onValueChange.Invoke(base.text);
				int caretSelectPosition = (base.caretPosition = Math.Min(num + input.Length, base.text.Length));
				m_CaretSelectPosition = caretSelectPosition;
			}
			else if (!base.multiLine && Application.platform == RuntimePlatform.OSXPlayer && IsModifier() && IsPaste())
			{
				string text = GetClipboard();
				foreach (char input2 in text)
				{
					base.Append(input2);
				}
			}
			else
			{
				base.Append(input);
			}
		}

		private string GetClipboard()
		{
			TextEditor textEditor = new TextEditor();
			textEditor.multiline = base.multiLine;
			textEditor.Paste();
			return textEditor.content.text;
		}
	}
}
