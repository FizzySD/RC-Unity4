using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using SimpleJSONFixed;

namespace Settings
{
	internal abstract class BaseSettingsContainer : BaseSetting
	{
		public OrderedDictionary Settings = new OrderedDictionary();

		public BaseSettingsContainer()
		{
			Setup();
		}

		protected virtual void Setup()
		{
			RegisterSettings();
			Apply();
		}

		protected void RegisterSettings()
		{
			foreach (FieldInfo item in from field in GetType().GetFields()
				where field.FieldType.IsSubclassOf(typeof(BaseSetting))
				select field)
			{
				Settings.Add(item.Name, (BaseSetting)item.GetValue(this));
			}
		}

		public override void SetDefault()
		{
			foreach (BaseSetting value in Settings.Values)
			{
				value.SetDefault();
			}
		}

		public virtual void Apply()
		{
		}

		public override JSONNode SerializeToJsonObject()
		{
			JSONObject jSONObject = new JSONObject();
			foreach (string key in Settings.Keys)
			{
				jSONObject.Add(key, ((BaseSetting)Settings[key]).SerializeToJsonObject());
			}
			return jSONObject;
		}

		public override void DeserializeFromJsonObject(JSONNode json)
		{
			JSONObject jSONObject = (JSONObject)json;
			foreach (string key in Settings.Keys)
			{
				if (jSONObject[key] != null)
				{
					((BaseSetting)Settings[key]).DeserializeFromJsonObject(jSONObject[key]);
				}
			}
			if (!Validate())
			{
				SetDefault();
			}
		}

		protected virtual bool Validate()
		{
			return true;
		}
	}
}
