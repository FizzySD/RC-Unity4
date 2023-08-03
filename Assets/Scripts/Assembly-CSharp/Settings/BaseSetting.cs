using SimpleJSONFixed;

namespace Settings
{
	internal abstract class BaseSetting
	{
		public abstract void SetDefault();

		public abstract JSONNode SerializeToJsonObject();

		public abstract void DeserializeFromJsonObject(JSONNode json);

		public virtual string SerializeToJsonString()
		{
			return SerializeToJsonObject().ToString(4);
		}

		public virtual void DeserializeFromJsonString(string json)
		{
			DeserializeFromJsonObject(JSON.Parse(json));
		}

		public virtual void Copy(BaseSetting other)
		{
			DeserializeFromJsonObject(other.SerializeToJsonObject());
		}
	}
}
