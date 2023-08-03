using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Localization")]
public class Localization : MonoBehaviour
{
	public TextAsset[] languages;

	private Dictionary<string, string> mDictionary = new Dictionary<string, string>();

	private static Localization mInstance;

	private string mLanguage;

	public string startingLanguage = "English";

	public string currentLanguage
	{
		get
		{
			return mLanguage;
		}
		set
		{
			if (!(mLanguage != value))
			{
				return;
			}
			startingLanguage = value;
			if (!string.IsNullOrEmpty(value))
			{
				if (languages != null)
				{
					int i = 0;
					for (int num = languages.Length; i < num; i++)
					{
						TextAsset textAsset = languages[i];
						if (textAsset != null && textAsset.name == value)
						{
							Load(textAsset);
							return;
						}
					}
				}
				TextAsset textAsset2 = Resources.Load(value, typeof(TextAsset)) as TextAsset;
				if (textAsset2 != null)
				{
					Load(textAsset2);
					return;
				}
			}
			mDictionary.Clear();
			PlayerPrefs.DeleteKey("Language");
		}
	}

	public static Localization instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = Object.FindObjectOfType(typeof(Localization)) as Localization;
				if (mInstance == null)
				{
					GameObject gameObject = new GameObject("_Localization");
					Object.DontDestroyOnLoad(gameObject);
					mInstance = gameObject.AddComponent<Localization>();
				}
			}
			return mInstance;
		}
	}

	public static bool isActive
	{
		get
		{
			return mInstance != null;
		}
	}

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			currentLanguage = PlayerPrefs.GetString("Language", startingLanguage);
			if (string.IsNullOrEmpty(mLanguage) && languages != null && languages.Length != 0)
			{
				currentLanguage = languages[0].name;
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public string Get(string key)
	{
		string value;
		if (mDictionary.TryGetValue(key, out value))
		{
			return value;
		}
		return key;
	}

	private void Load(TextAsset asset)
	{
		mLanguage = asset.name;
		PlayerPrefs.SetString("Language", mLanguage);
		mDictionary = new ByteReader(asset).ReadDictionary();
		UIRoot.Broadcast("OnLocalize", this);
	}

	public static string Localize(string key)
	{
		if (!(instance == null))
		{
			return instance.Get(key);
		}
		return key;
	}

	private void OnDestroy()
	{
		if (mInstance == this)
		{
			mInstance = null;
		}
	}

	private void OnEnable()
	{
		if (mInstance == null)
		{
			mInstance = this;
		}
	}
}
