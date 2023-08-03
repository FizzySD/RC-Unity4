using UnityEngine;

[AddComponentMenu("NGUI/UI/Input (Basic)")]
public class UIInput : MonoBehaviour
{
	public enum KeyboardType
	{
		Default = 0,
		ASCIICapable = 1,
		NumbersAndPunctuation = 2,
		URL = 3,
		NumberPad = 4,
		PhonePad = 5,
		NamePhonePad = 6,
		EmailAddress = 7
	}

	public delegate void OnSubmit(string inputString);

	public delegate char Validator(string currentText, char nextChar);

	public Color activeColor = Color.white;

	public bool autoCorrect;

	public string caratChar = "|";

	public static UIInput current;

	public GameObject eventReceiver;

	public string functionName = "OnSubmit";

	public bool isPassword;

	public UILabel label;

	public int maxChars;

	private Color mDefaultColor = Color.white;

	private string mDefaultText = string.Empty;

	private bool mDoInit = true;

	private string mLastIME = string.Empty;

	private UIWidget.Pivot mPivot = UIWidget.Pivot.Left;

	private float mPosition;

	private string mText = string.Empty;

	public OnSubmit onSubmit;

	public GameObject selectOnTab;

	public KeyboardType type;

	public bool useLabelTextAtStart;

	public Validator validator;

	public string defaultText
	{
		get
		{
			return mDefaultText;
		}
		set
		{
			if (label.text == mDefaultText)
			{
				label.text = value;
			}
			mDefaultText = value;
		}
	}

	public bool selected
	{
		get
		{
			return UICamera.selectedObject == base.gameObject;
		}
		set
		{
			if (!value && UICamera.selectedObject == base.gameObject)
			{
				UICamera.selectedObject = null;
			}
			else if (value)
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	public virtual string text
	{
		get
		{
			if (mDoInit)
			{
				initMain();
			}
			return mText;
		}
		set
		{
			if (mDoInit)
			{
				initMain();
			}
			mText = value;
			if (label != null)
			{
				if (string.IsNullOrEmpty(value))
				{
					value = mDefaultText;
				}
				label.supportEncoding = false;
				label.text = ((!selected) ? value : (value + caratChar));
				label.showLastPasswordChar = selected;
				label.color = ((!selected && !(value != mDefaultText)) ? mDefaultColor : activeColor);
			}
		}
	}

	private void Append(string input)
	{
		int i = 0;
		for (int length = input.Length; i < length; i++)
		{
			char c = input[i];
			if (c != '\b')
			{
				if (c == '\n' || c == '\r')
				{
					if ((UICamera.current.submitKey0 == KeyCode.Return || UICamera.current.submitKey1 == KeyCode.Return) && (!label.multiLine || (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))))
					{
						current = this;
						if (onSubmit != null)
						{
							onSubmit(mText);
						}
						if (eventReceiver == null)
						{
							eventReceiver = base.gameObject;
						}
						eventReceiver.SendMessage(functionName, mText, SendMessageOptions.DontRequireReceiver);
						current = null;
						selected = false;
						return;
					}
					if (validator != null)
					{
						c = validator(mText, c);
					}
					if (c == '\0')
					{
						continue;
					}
					if (c == '\n' || c == '\r')
					{
						if (label.multiLine)
						{
							mText += "\n";
						}
					}
					else
					{
						mText += c;
					}
					SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
				}
				else if (c >= ' ')
				{
					if (validator != null)
					{
						c = validator(mText, c);
					}
					if (c != 0)
					{
						mText += c;
						SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else if (mText.Length > 0)
			{
				mText = mText.Substring(0, mText.Length - 1);
				SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
			}
		}
		UpdateLabel();
	}

	protected void Init()
	{
		maxChars = 100;
		initMain();
	}

	protected void initMain()
	{
		maxChars = 100;
		if (!mDoInit)
		{
			return;
		}
		mDoInit = false;
		if (label == null)
		{
			label = GetComponentInChildren<UILabel>();
		}
		if (label != null)
		{
			if (useLabelTextAtStart)
			{
				mText = label.text;
			}
			mDefaultText = label.text;
			mDefaultColor = label.color;
			label.supportEncoding = false;
			label.password = isPassword;
			mPivot = label.pivot;
			mPosition = label.cachedTransform.localPosition.x;
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		if (UICamera.IsHighlighted(base.gameObject))
		{
			OnSelect(false);
		}
	}

	private void OnEnable()
	{
		if (UICamera.IsHighlighted(base.gameObject))
		{
			OnSelect(true);
		}
	}

	private void OnInput(string input)
	{
		if (mDoInit)
		{
			initMain();
		}
		if (selected && base.enabled && NGUITools.GetActive(base.gameObject) && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			Append(input);
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (mDoInit)
		{
			initMain();
		}
		if (!(label != null) || !base.enabled || !NGUITools.GetActive(base.gameObject))
		{
			return;
		}
		if (isSelected)
		{
			mText = ((useLabelTextAtStart || !(label.text == mDefaultText)) ? label.text : string.Empty);
			label.color = activeColor;
			if (isPassword)
			{
				label.password = true;
			}
			Input.imeCompositionMode = IMECompositionMode.On;
			Transform cachedTransform = label.cachedTransform;
			Vector3 position = label.pivotOffset;
			position.y += label.relativeSize.y;
			position = cachedTransform.TransformPoint(position);
			Input.compositionCursorPos = UICamera.currentCamera.WorldToScreenPoint(position);
			UpdateLabel();
			return;
		}
		if (string.IsNullOrEmpty(mText))
		{
			label.text = mDefaultText;
			label.color = mDefaultColor;
			if (isPassword)
			{
				label.password = false;
			}
		}
		else
		{
			label.text = mText;
		}
		label.showLastPasswordChar = false;
		Input.imeCompositionMode = IMECompositionMode.Off;
		RestoreLabel();
	}

	private void RestoreLabel()
	{
		if (label != null)
		{
			label.pivot = mPivot;
			Vector3 localPosition = label.cachedTransform.localPosition;
			localPosition.x = mPosition;
			label.cachedTransform.localPosition = localPosition;
		}
	}

	private void Update()
	{
		if (selected)
		{
			if (selectOnTab != null && Input.GetKeyDown(KeyCode.Tab))
			{
				UICamera.selectedObject = selectOnTab;
			}
			if (Input.GetKeyDown(KeyCode.V) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
			{
				Append(NGUITools.clipboard);
			}
			if (mLastIME != Input.compositionString)
			{
				mLastIME = Input.compositionString;
				UpdateLabel();
			}
		}
	}

	private void UpdateLabel()
	{
		if (mDoInit)
		{
			initMain();
		}
		if (maxChars > 0 && mText.Length > maxChars)
		{
			mText = mText.Substring(0, maxChars);
		}
		if (!(label.font != null))
		{
			return;
		}
		string text;
		if (isPassword && selected)
		{
			text = string.Empty;
			int i = 0;
			for (int length = mText.Length; i < length; i++)
			{
				text += "*";
			}
			text = text + Input.compositionString + caratChar;
		}
		else
		{
			text = ((!selected) ? mText : (mText + Input.compositionString + caratChar));
		}
		label.supportEncoding = false;
		if (!label.shrinkToFit)
		{
			if (label.multiLine)
			{
				text = label.font.WrapText(text, (float)label.lineWidth / label.cachedTransform.localScale.x, 0, false, UIFont.SymbolStyle.None);
			}
			else
			{
				string endOfLineThatFits = label.font.GetEndOfLineThatFits(text, (float)label.lineWidth / label.cachedTransform.localScale.x, false, UIFont.SymbolStyle.None);
				if (endOfLineThatFits != text)
				{
					text = endOfLineThatFits;
					Vector3 localPosition = label.cachedTransform.localPosition;
					localPosition.x = mPosition + (float)label.lineWidth;
					if (mPivot == UIWidget.Pivot.Left)
					{
						label.pivot = UIWidget.Pivot.Right;
					}
					else if (mPivot == UIWidget.Pivot.TopLeft)
					{
						label.pivot = UIWidget.Pivot.TopRight;
					}
					else if (mPivot == UIWidget.Pivot.BottomLeft)
					{
						label.pivot = UIWidget.Pivot.BottomRight;
					}
					label.cachedTransform.localPosition = localPosition;
				}
				else
				{
					RestoreLabel();
				}
			}
		}
		label.text = text;
		label.showLastPasswordChar = selected;
	}
}
