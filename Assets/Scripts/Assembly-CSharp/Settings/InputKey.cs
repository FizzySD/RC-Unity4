using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
	internal class InputKey
	{
		protected KeyCode _key;

		protected bool _isSpecial;

		protected SpecialKey _special;

		protected bool _isModifier;

		protected KeyCode _modifier;

		protected HashSet<KeyCode> ModifierKeys = new HashSet<KeyCode>
		{
			KeyCode.LeftShift,
			KeyCode.LeftAlt,
			KeyCode.LeftControl,
			KeyCode.RightShift,
			KeyCode.RightAlt,
			KeyCode.RightControl
		};

		protected HashSet<string> AlphaDigits = new HashSet<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

		public InputKey()
		{
		}

		public InputKey(string keyStr)
		{
			LoadFromString(keyStr);
		}

		public bool MatchesKeyCode(KeyCode key)
		{
			if (!_isSpecial && !_isModifier)
			{
				return _key == key;
			}
			return false;
		}

		public bool ReadNextInput()
		{
			_isModifier = false;
			foreach (KeyCode modifierKey in ModifierKeys)
			{
				if (Input.GetKey(modifierKey))
				{
					_modifier = modifierKey;
					_isModifier = true;
				}
			}
			foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
			{
				if (ModifierKeys.Contains(value) && Input.GetKeyUp(value))
				{
					_isModifier = false;
					_key = value;
					_isSpecial = false;
					return true;
				}
				if (!ModifierKeys.Contains(value) && value == KeyCode.Mouse0 && Input.GetKeyUp(value))
				{
					_key = value;
					_isSpecial = false;
					return true;
				}
				if (!ModifierKeys.Contains(value) && value != KeyCode.Mouse0 && Input.GetKeyDown(value))
				{
					_key = value;
					_isSpecial = false;
					return true;
				}
			}
			foreach (SpecialKey value2 in Enum.GetValues(typeof(SpecialKey)))
			{
				if (GetSpecial(value2))
				{
					_special = value2;
					_isSpecial = true;
					return true;
				}
			}
			return false;
		}

		public bool GetKeyDown()
		{
			if (_isSpecial)
			{
				if (GetModifier())
				{
					return GetSpecial(_special);
				}
				return false;
			}
			if (GetModifier())
			{
				return Input.GetKeyDown(_key);
			}
			return false;
		}

		public bool GetKey()
		{
			if (_isSpecial)
			{
				if (GetModifier())
				{
					return GetSpecial(_special);
				}
				return false;
			}
			if (GetModifier())
			{
				return Input.GetKey(_key);
			}
			return false;
		}

		public bool GetKeyUp()
		{
			if (_isSpecial)
			{
				if (GetModifier())
				{
					return GetSpecial(_special);
				}
				return false;
			}
			if (GetModifier())
			{
				return Input.GetKeyUp(_key);
			}
			return false;
		}

		public bool IsWheel()
		{
			if (_isSpecial)
			{
				if (_special != SpecialKey.WheelDown)
				{
					return _special == SpecialKey.WheelUp;
				}
				return true;
			}
			return false;
		}

		public bool IsNone()
		{
			if (_isSpecial)
			{
				return _special == SpecialKey.None;
			}
			return false;
		}

		public override string ToString()
		{
			string text = (_isSpecial ? _special.ToString() : _key.ToString());
			if (text.StartsWith("Alpha"))
			{
				text = text.Substring(5);
			}
			if (_isModifier)
			{
				text = _modifier.ToString() + "+" + text;
			}
			return text;
		}

		public override bool Equals(object obj)
		{
			return ToString() == obj.ToString();
		}

		public void LoadFromString(string serializedKey)
		{
			_isModifier = false;
			string[] array = serializedKey.Split('+');
			string text = array[0];
			if (array.Length > 1)
			{
				_modifier = text.ToEnum<KeyCode>();
				_isModifier = true;
				text = array[1];
			}
			if (text.Length == 1 && AlphaDigits.Contains(text))
			{
				text = "Alpha" + text;
			}
			KeyCode keyCode = text.ToEnum<KeyCode>();
			if (keyCode != 0)
			{
				_key = keyCode;
				_isSpecial = false;
			}
			else
			{
				_special = text.ToEnum<SpecialKey>();
				_isSpecial = true;
			}
		}

		protected bool GetModifier()
		{
			if (_isModifier)
			{
				return Input.GetKey(_modifier);
			}
			return true;
		}

		protected bool GetSpecial(SpecialKey specialKey)
		{
			switch (specialKey)
			{
			case SpecialKey.WheelUp:
				return Input.GetAxis("Mouse ScrollWheel") > 0f;
			case SpecialKey.WheelDown:
				return Input.GetAxis("Mouse ScrollWheel") < 0f;
			default:
				return false;
			}
		}
	}
}
